using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPack1
{
	private class Pack1 : IDisposable, IPackageDisposable, IPackage, IBaseDisposable
	{
		private IPackageFileDisposable _lastFile;

		private int _lastFileIndex = -1;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "Pack1";

		private GetPack1FileCDelegate GetPack1FileC { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public BytesCount TotalBytes { get; }

		private ImmutableArray<AppRemotePackageFileInfo> FilesInfo { get; }

		private AppRemotePack1UnpackInfo UnpackInfo { get; }

		private IPackageStream Stream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
			_lastFile?.Dispose();
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
			if (_lastFile != null)
			{
				_lastFile.Dispose();
				_lastFile = null;
			}
			while (_lastFile == null)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (_lastFileIndex + 1 < UnpackInfo.FilesInfo.Length)
				{
					_lastFileIndex++;
					AppRemotePack1FileUnpackInfo unpackFileInfo = UnpackInfo.FilesInfo[_lastFileIndex];
					AppRemotePackageFileInfo? appRemotePackageFileInfo = FilesInfo.FirstOrNull((AppRemotePackageFileInfo x) => (string)x.Path == (string)unpackFileInfo.Path);
					if (appRemotePackageFileInfo.HasValue)
					{
						_lastFile = GetPack1FileC(appRemotePackageFileInfo.Value, unpackInfo: unpackFileInfo, encryption: UnpackInfo.Encryption, compressionMethod: UnpackInfo.CompressionMethod, password: UnpackInfo.Password, packageStream: Stream, cancellationToken: cancellationToken, onPackageFileSourceStreamOutOfRange: (PackageFileSourceStreamOutOfRange e) =>
						{
							throw new UnpredictedException(e);
						}, onInvalidAppAuthKey: null, onExpiredAppAuthKey: null);
						Assert.IsNotNullFromDelegate(_lastFile);
						return _lastFile;
					}
					continue;
				}
				return null;
			}
			return null;
		}

		public IPackageFileDisposable GetNextFile(CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
		{
			_disposalHandler.ThrowIfDisposed();
			LogInfo("Executing Pack1::GetNextFile...");
			LogTrace("cancellationToken = " + cancellationToken);
			try
			{
				BeginLogRegion(enabled: true);
				IPackageFileDisposable packageFileDisposable = ExecuteGetNextFile(cancellationToken);
				EndLogRegion(enabled: true);
				LogTrace("result = " + packageFileDisposable);
				LogInfo("Pack1::GetNextFile finished: success.");
				return packageFileDisposable;
			}
			catch (InvalidAppAuthKey e)
			{
				EndLogRegion(enabled: true);
				LogInfo("Pack1::GetNextFile finished: InvalidAppAuthKey.");
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
				LogInfo("Pack1::GetNextFile finished: ExpiredAppAuthKey.");
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
				LogInfo("Pack1::GetNextFile has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				EndLogRegion(enabled: true);
				LogError("Pack1::GetNextFile failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				EndLogRegion(enabled: true);
				LogError("Pack1::GetNextFile finished with fatal error.", ex2);
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
				LogInfo("Pack1::GetProcessedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("Pack1::GetProcessedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("Pack1::GetProcessedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public Pack1(BytesCount totalBytes, ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemotePack1UnpackInfo unpackInfo, IPackageStream stream, GetPack1FileCDelegate getPack1FileC, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(totalBytes, "totalBytes");
			Arg.IsValid(filesInfo, "filesInfo");
			Arg.IsValid(unpackInfo, "unpackInfo");
			Arg.IsNotNull(stream, "stream");
			Arg.IsNotNull(getPack1FileC, "getPack1FileC");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			TotalBytes = totalBytes;
			FilesInfo = filesInfo;
			UnpackInfo = unpackInfo;
			Stream = stream;
			GetPack1FileC = getPack1FileC;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~Pack1()
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

	public GetPack1FileCDelegate GetPack1FileC { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageDisposable Execute(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemotePack1UnpackInfo unpackInfo, IPackageStream stream, BytesCount size)
	{
		return NewPack1(size, filesInfo, unpackInfo, stream);
	}

	private Pack1 NewPack1(BytesCount totalBytes, ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemotePack1UnpackInfo unpackInfo, IPackageStream stream)
	{
		return new Pack1(totalBytes, filesInfo, unpackInfo, stream, GetPack1FileC, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPack1(GetPack1FileCDelegate getPack1FileC, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getPack1FileC);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetPack1FileC = getPack1FileC;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageDisposable DelegateExecute(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemotePack1UnpackInfo unpackInfo, IPackageStream stream, BytesCount size)
	{
		Arg.IsValid(filesInfo, "filesInfo");
		Arg.IsValid(unpackInfo, "unpackInfo");
		Arg.IsNotNull(stream, "stream");
		Arg.IsValid(size, "size");
		LogInfo("Executing GetPack1...");
		ImmutableArray<AppRemotePackageFileInfo> immutableArray = filesInfo;
		LogTrace("filesInfo = " + immutableArray.ToString());
		AppRemotePack1UnpackInfo appRemotePack1UnpackInfo = unpackInfo;
		LogTrace("unpackInfo = " + appRemotePack1UnpackInfo.ToString());
		LogTrace("stream = " + stream);
		BytesCount bytesCount = size;
		LogTrace("size = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageDisposable packageDisposable = Execute(filesInfo, unpackInfo, stream, size);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageDisposable);
			LogInfo("GetPack1 finished: success.");
			return packageDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1 has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1 failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1 finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPack1Delegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPack1(context.Resolve<GetPack1FileCDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
