using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using ICSharpCode.SharpZipLib.Zip;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetZipFile
{
	private class ZipFileStream : IDisposable, IPackageFileStreamDisposable, IPackageFileStream, IBaseDisposable
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "ZipFileStream";

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private ZipInputStream Stream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer, CancellationToken cancellationToken)
		{
			int v = Stream.Read(buffer.Array, buffer.Offset, buffer.Length);
			return new StreamReadResult(new StreamReadLength(v));
		}

		public StreamReadResult Read(Buffer<byte> buffer, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
		{
			_disposalHandler.ThrowIfDisposed();
			Arg.IsValid(buffer, "buffer");
			try
			{
				return ExecuteRead(buffer, cancellationToken);
			}
			catch (InvalidAppAuthKey e)
			{
				if (onInvalidAppAuthKey == null)
				{
					throw;
				}
				object obj = onInvalidAppAuthKey(e);
				if (obj == null)
				{
					return default;
				}
				return (StreamReadResult)obj;
			}
			catch (ExpiredAppAuthKey e2)
			{
				if (onExpiredAppAuthKey == null)
				{
					throw;
				}
				object obj2 = onExpiredAppAuthKey(e2);
				if (obj2 == null)
				{
					return default;
				}
				return (StreamReadResult)obj2;
			}
			catch (OperationCanceledException)
			{
				LogInfo("ZipFileStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("ZipFileStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("ZipFileStream::Read finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public ZipFileStream(ZipInputStream stream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(stream, "stream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Stream = stream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~ZipFileStream()
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

	private class ZipDataFile : IDisposable, IPackageDataFileDisposable, IPackageFileDisposable, IPackageFile, IBaseDisposable, IPackageDataFile
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "ZipDataFile";

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public AppPath Path { get; }

		public AppVersionId VersionId { get; }

		public IPackageFileStream Stream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		public ZipDataFile(AppPath path, AppVersionId versionId, IPackageFileStream stream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(path, "path");
			Arg.IsValid(versionId, "versionId");
			Arg.IsNotNull(stream, "stream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Path = path;
			VersionId = versionId;
			Stream = stream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~ZipDataFile()
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

	private class ZipPatchFile : IDisposable, IPackagePatchFileDisposable, IPackageFileDisposable, IPackageFile, IBaseDisposable, IPackagePatchFile
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "ZipPatchFile";

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public AppPath Path { get; }

		public AppVersionId SourceVersionId { get; }

		public AppVersionId TargetVersionId { get; }

		public bool IsPatchEmpty { get; }

		public IPackageFileStream Stream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		public ZipPatchFile(AppPath path, AppVersionId sourceVersionId, AppVersionId targetVersionId, bool isPatchEmpty, IPackageFileStream stream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(path, "path");
			Arg.IsValid(sourceVersionId, "sourceVersionId");
			Arg.IsValid(targetVersionId, "targetVersionId");
			Arg.IsNotNull(stream, "stream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Path = path;
			SourceVersionId = sourceVersionId;
			TargetVersionId = targetVersionId;
			IsPatchEmpty = isPatchEmpty;
			Stream = stream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~ZipPatchFile()
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

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageFileDisposable Execute(AppRemotePackageFileInfo info, ZipInputStream stream)
	{
		ZipFileStream zipFileStream = NewZipFileStream(stream);
		IPackageFileDisposable packageFileDisposable = null;
		if (info.DataFileInfo.HasValue)
		{
			AppRemotePackageDataFileInfo value = info.DataFileInfo.Value;
			packageFileDisposable = NewZipDataFile(value.Path, value.VersionId, zipFileStream);
		}
		if (info.PatchFileInfo.HasValue)
		{
			AppRemotePackagePatchFileInfo value2 = info.PatchFileInfo.Value;
			packageFileDisposable = NewZipPatchFile(value2.Path, value2.SourceVersionId, value2.TargetVersionId, value2.IsPatchEmpty, zipFileStream);
		}
		Assert.IsNotNull(packageFileDisposable);
		packageFileDisposable.AttachDisposable(zipFileStream);
		return packageFileDisposable;
	}

	private ZipFileStream NewZipFileStream(ZipInputStream stream)
	{
		return new ZipFileStream(stream, Log, BeginLogRegion, EndLogRegion);
	}

	private ZipDataFile NewZipDataFile(AppPath path, AppVersionId versionId, IPackageFileStream stream)
	{
		return new ZipDataFile(path, versionId, stream, Log, BeginLogRegion, EndLogRegion);
	}

	private ZipPatchFile NewZipPatchFile(AppPath path, AppVersionId sourceVersionId, AppVersionId targetVersionId, bool isPatchEmpty, IPackageFileStream stream)
	{
		return new ZipPatchFile(path, sourceVersionId, targetVersionId, isPatchEmpty, stream, Log, BeginLogRegion, EndLogRegion);
	}

	public GetZipFile(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageFileDisposable DelegateExecute(AppRemotePackageFileInfo info, ZipInputStream stream)
	{
		Arg.IsValid(info, "info");
		Arg.IsNotNull(stream, "stream");
		LogInfo("Executing GetZipFile...");
		AppRemotePackageFileInfo appRemotePackageFileInfo = info;
		LogTrace("info = " + appRemotePackageFileInfo.ToString());
		LogTrace("stream = " + stream);
		try
		{
			BeginLogRegion(enabled: true);
			IPackageFileDisposable packageFileDisposable = Execute(info, stream);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageFileDisposable);
			LogInfo("GetZipFile finished: success.");
			return packageFileDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetZipFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetZipFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetZipFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetZipFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetZipFile(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
