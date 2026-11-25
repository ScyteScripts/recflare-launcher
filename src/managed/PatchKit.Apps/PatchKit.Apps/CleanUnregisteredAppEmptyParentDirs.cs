using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class CleanUnregisteredAppEmptyParentDirs
{
	public DeleteDirIfExistsDelegate DeleteDirIfExists { get; }

	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public GetParentAppPathDelegate GetParentAppPath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path)
	{
		bool lastDirNotEmpty = false;
		AppPath? appPath = path;
		while (!lastDirNotEmpty)
		{
			appPath = GetParentAppPath(appPath.Value);
			if (!appPath.HasValue || GetAppEntryInfo(app, appPath.Value, null).HasValue)
			{
				break;
			}
			Path path2 = GetAppEntryFullPath(app, appPath.Value);
			DeleteDirIfExists(path2, null, (NotEmptyDir e) =>
			{
				lastDirNotEmpty = true;
			});
		}
	}

	public CleanUnregisteredAppEmptyParentDirs(DeleteDirIfExistsDelegate deleteDirIfExists, GetAppEntryInfoDelegate getAppEntryInfo, GetAppEntryFullPathDelegate getAppEntryFullPath, GetParentAppPathDelegate getParentAppPath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(deleteDirIfExists);
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(getParentAppPath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DeleteDirIfExists = deleteDirIfExists;
		GetAppEntryInfo = getAppEntryInfo;
		GetAppEntryFullPath = getAppEntryFullPath;
		GetParentAppPath = getParentAppPath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing CleanUnregisteredAppEmptyParentDirs...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path);
			EndLogRegion(enabled: true);
			LogInfo("CleanUnregisteredAppEmptyParentDirs finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("CleanUnregisteredAppEmptyParentDirs finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("CleanUnregisteredAppEmptyParentDirs has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CleanUnregisteredAppEmptyParentDirs failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CleanUnregisteredAppEmptyParentDirs finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CleanUnregisteredAppEmptyParentDirsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CleanUnregisteredAppEmptyParentDirs(context.Resolve<DeleteDirIfExistsDelegate>(), context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<GetParentAppPathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
