using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Autofac;
using ICSharpCode.SharpZipLib.Zip;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetZip
{
	private class ZipFileStream : Stream
	{
		private readonly IPackageStream _packageStream;

		public override long Length => _packageStream.Length;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Position
		{
			get
			{
				throw new UnpredictedCondition("Get Position shouldn't be called.");
			}
			set
			{
				throw new UnpredictedCondition("Set Position shouldn't be called.");
			}
		}

		public ZipFileStream(IPackageStream packageStream)
		{
			_packageStream = packageStream;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _packageStream.Read(new Buffer<byte>(buffer, new BufferOffset(offset), new BufferLength(count)), CancellationToken.None, null, null).Length;
		}

		public override void Flush()
		{
			throw new UnpredictedCondition("Flush shouldn't be called.");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new UnpredictedCondition("Seek shouldn't be called.");
		}

		public override void SetLength(long value)
		{
			throw new UnpredictedCondition("SetLength shouldn't be called.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new UnpredictedCondition("Write shouldn't be called.");
		}
	}

	private class Zip : IDisposable, IPackageDisposable, IPackage, IBaseDisposable
	{
		private ZipInputStream _zipInputStream;

		private ZipFileStream _zipFileStream;

		private IPackageFileDisposable _lastFile;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "Zip";

		private GetZipFileDelegate GetZipFile { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public BytesCount TotalBytes { get; }

		private ImmutableArray<AppRemotePackageFileInfo> FilesInfo { get; }

		private AppRemoteZipUnpackInfo UnpackInfo { get; }

		private IPackageStream Stream { get; }

		private void Constructor()
		{
			_zipFileStream = new ZipFileStream(Stream);
			_zipInputStream = new ZipInputStream(_zipFileStream)
			{
				Password = UnpackInfo.Password
			};
		}

		private void DisposeManaged()
		{
			Assert.IsNotNull(_zipInputStream);
			Assert.IsNotNull(_zipFileStream);
			_lastFile?.Dispose();
			_zipInputStream.Dispose();
			_zipFileStream.Dispose();
		}

		private void DisposeUnmanaged()
		{
		}

		private BytesCount ExecuteGetProcessedBytes()
		{
			return Stream.GetCurrentPosition() + Stream.GetBufferedBytes();
		}

		private IPackageFileDisposable ExecuteGetNextFile(CancellationToken cancellationToken)
		{
			Assert.IsNotNull(_zipInputStream);
			if (_lastFile != null)
			{
				_lastFile.Dispose();
				_lastFile = null;
			}
			while (_lastFile == null)
			{
				ZipEntry nextEntry = _zipInputStream.GetNextEntry();
				if (nextEntry == null)
				{
					return null;
				}
				Assert.IsNotNull(nextEntry.Name);
				AppPath path = new AppPath(nextEntry.Name.TrimEnd('\\', '/'));
				if (nextEntry.IsFile)
				{
					AppRemotePackageFileInfo? appRemotePackageFileInfo = FilesInfo.FirstOrNull((AppRemotePackageFileInfo x) => (string)x.Path == (string)path);
					if (appRemotePackageFileInfo.HasValue)
					{
						_lastFile = GetZipFile(appRemotePackageFileInfo.Value, _zipInputStream);
						Assert.IsNotNullFromDelegate(_lastFile);
					}
				}
			}
			return _lastFile;
		}

		public IPackageFileDisposable GetNextFile(CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
		{
			_disposalHandler.ThrowIfDisposed();
			LogInfo("Executing Zip::GetNextFile...");
			LogTrace("cancellationToken = " + cancellationToken);
			try
			{
				BeginLogRegion(enabled: true);
				IPackageFileDisposable packageFileDisposable = ExecuteGetNextFile(cancellationToken);
				EndLogRegion(enabled: true);
				LogTrace("result = " + packageFileDisposable);
				LogInfo("Zip::GetNextFile finished: success.");
				return packageFileDisposable;
			}
			catch (InvalidAppAuthKey e)
			{
				EndLogRegion(enabled: true);
				LogInfo("Zip::GetNextFile finished: InvalidAppAuthKey.");
				if (onInvalidAppAuthKey == null)
				{
					throw;
				}
				object obj = onInvalidAppAuthKey(e);
				if (obj == null)
				{
					return null;
				}
				return (IPackageFileDisposable)obj;
			}
			catch (ExpiredAppAuthKey e2)
			{
				EndLogRegion(enabled: true);
				LogInfo("Zip::GetNextFile finished: ExpiredAppAuthKey.");
				if (onExpiredAppAuthKey == null)
				{
					throw;
				}
				object obj2 = onExpiredAppAuthKey(e2);
				if (obj2 == null)
				{
					return null;
				}
				return (IPackageFileDisposable)obj2;
			}
			catch (OperationCanceledException)
			{
				EndLogRegion(enabled: true);
				LogInfo("Zip::GetNextFile has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				EndLogRegion(enabled: true);
				LogError("Zip::GetNextFile failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				EndLogRegion(enabled: true);
				LogError("Zip::GetNextFile finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public BytesCount GetProcessedBytes()
		{
			_disposalHandler.ThrowIfDisposed();
			try
			{
				return ExecuteGetProcessedBytes();
			}
			catch (OperationCanceledException)
			{
				LogInfo("Zip::GetProcessedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("Zip::GetProcessedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("Zip::GetProcessedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public Zip(BytesCount totalBytes, ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemoteZipUnpackInfo unpackInfo, IPackageStream stream, GetZipFileDelegate getZipFile, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(totalBytes, "totalBytes");
			Arg.IsValid(filesInfo, "filesInfo");
			Arg.IsValid(unpackInfo, "unpackInfo");
			Arg.IsNotNull(stream, "stream");
			Arg.IsNotNull(getZipFile, "getZipFile");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			TotalBytes = totalBytes;
			FilesInfo = filesInfo;
			UnpackInfo = unpackInfo;
			Stream = stream;
			GetZipFile = getZipFile;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~Zip()
		{
			Dispose(isDisposing: false);
		}

		public void Dispose()
		{
			Dispose(isDisposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool isDisposing)
		{
			if (_disposalHandler.IsDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				DisposeManaged();
				if (_attachedDisposables != null)
				{
					foreach (IDisposable attachedDisposable in _attachedDisposables)
					{
						attachedDisposable.Dispose();
					}
				}
			}
			DisposeUnmanaged();
			_disposalHandler.MarkAsDisposed();
		}

		public void AttachDisposable(IDisposable disposable)
		{
			Assert.IsNotNull(disposable);
			if (_attachedDisposables == null)
			{
				_attachedDisposables = new List<IDisposable>();
			}
			_attachedDisposables.Add(disposable);
		}

		private void LogTrace(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Trace, exception));
		}

		private void LogInfo(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Info, exception));
		}

		private void LogWarning(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Warning, exception));
		}

		private void LogError(string message, Exception exception = null)
		{
			Log(new LogMessage(message, LogMessageType.Error, exception));
		}
	}

	public GetZipFileDelegate GetZipFile { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageDisposable Execute(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemoteZipUnpackInfo unpackInfo, IPackageStream stream, BytesCount size)
	{
		return NewZip(size, filesInfo, unpackInfo, stream);
	}

	private Zip NewZip(BytesCount totalBytes, ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemoteZipUnpackInfo unpackInfo, IPackageStream stream)
	{
		return new Zip(totalBytes, filesInfo, unpackInfo, stream, GetZipFile, Log, BeginLogRegion, EndLogRegion);
	}

	public GetZip(GetZipFileDelegate getZipFile, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getZipFile);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetZipFile = getZipFile;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageDisposable DelegateExecute(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemoteZipUnpackInfo unpackInfo, IPackageStream stream, BytesCount size)
	{
		Arg.IsValid(filesInfo, "filesInfo");
		Arg.IsValid(unpackInfo, "unpackInfo");
		Arg.IsNotNull(stream, "stream");
		Arg.IsValid(size, "size");
		LogInfo("Executing GetZip...");
		ImmutableArray<AppRemotePackageFileInfo> immutableArray = filesInfo;
		LogTrace("filesInfo = " + immutableArray.ToString());
		AppRemoteZipUnpackInfo appRemoteZipUnpackInfo = unpackInfo;
		LogTrace("unpackInfo = " + appRemoteZipUnpackInfo.ToString());
		LogTrace("stream = " + stream);
		BytesCount bytesCount = size;
		LogTrace("size = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageDisposable packageDisposable = Execute(filesInfo, unpackInfo, stream, size);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageDisposable);
			LogInfo("GetZip finished: success.");
			return packageDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetZip has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetZip failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetZip finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetZipDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetZip(context.Resolve<GetZipFileDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
	}

	private void LogTrace(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Trace, exception));
	}

	private void LogInfo(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Info, exception));
	}

	private void LogWarning(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Warning, exception));
	}

	private void LogError(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Error, exception));
	}
}
