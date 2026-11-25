using System;
using System.Collections.Generic;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPack1FileA
{
	private class Pack1DataFile : IDisposable, IPackageDataFileDisposable, IPackageFileDisposable, IPackageFile, IBaseDisposable, IPackageDataFile
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "Pack1DataFile";

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

		public Pack1DataFile(AppPath path, AppVersionId versionId, IPackageFileStream stream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

		~Pack1DataFile()
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

	private class Pack1PatchFile : IDisposable, IPackagePatchFileDisposable, IPackageFileDisposable, IPackageFile, IBaseDisposable, IPackagePatchFile
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "Pack1PatchFile";

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

		public Pack1PatchFile(AppPath path, AppVersionId sourceVersionId, AppVersionId targetVersionId, bool isPatchEmpty, IPackageFileStream stream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

		~Pack1PatchFile()
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

	private IPackageFileDisposable Execute(AppRemotePackageFileInfo info, IPackageFileStream stream)
	{
		IPackageFileDisposable packageFileDisposable = null;
		if (info.DataFileInfo.HasValue)
		{
			AppRemotePackageDataFileInfo value = info.DataFileInfo.Value;
			packageFileDisposable = NewPack1DataFile(value.Path, value.VersionId, stream);
		}
		if (info.PatchFileInfo.HasValue)
		{
			AppRemotePackagePatchFileInfo value2 = info.PatchFileInfo.Value;
			packageFileDisposable = NewPack1PatchFile(value2.Path, value2.SourceVersionId, value2.TargetVersionId, value2.IsPatchEmpty, stream);
		}
		Assert.IsNotNull(packageFileDisposable);
		return packageFileDisposable;
	}

	private Pack1DataFile NewPack1DataFile(AppPath path, AppVersionId versionId, IPackageFileStream stream)
	{
		return new Pack1DataFile(path, versionId, stream, Log, BeginLogRegion, EndLogRegion);
	}

	private Pack1PatchFile NewPack1PatchFile(AppPath path, AppVersionId sourceVersionId, AppVersionId targetVersionId, bool isPatchEmpty, IPackageFileStream stream)
	{
		return new Pack1PatchFile(path, sourceVersionId, targetVersionId, isPatchEmpty, stream, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPack1FileA(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageFileDisposable DelegateExecute(AppRemotePackageFileInfo info, IPackageFileStream stream)
	{
		Arg.IsValid(info, "info");
		Arg.IsNotNull(stream, "stream");
		LogInfo("Executing GetPack1FileA...");
		AppRemotePackageFileInfo appRemotePackageFileInfo = info;
		LogTrace("info = " + appRemotePackageFileInfo.ToString());
		LogTrace("stream = " + stream);
		try
		{
			BeginLogRegion(enabled: true);
			IPackageFileDisposable packageFileDisposable = Execute(info, stream);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageFileDisposable);
			LogInfo("GetPack1FileA finished: success.");
			return packageFileDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileA has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileA failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileA finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPack1FileADelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPack1FileA(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
