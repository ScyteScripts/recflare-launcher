using System;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class StartApp
{
	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public IsAppInstalledDelegate IsAppInstalled { get; }

	public DoesAppNeedRepairDelegate DoesAppNeedRepair { get; }

	public GetAppStartInfoDelegate GetAppStartInfo { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public GetParentPathDelegate GetParentPath { get; }

	public StartProcessDelegate StartProcess { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ProcessId Execute(App app, Text? appProcessExtraArgsAsText)
	{
		if (!DoesAppRootDirExist(app, null))
		{
			throw new AppRootDirDoesNotExist();
		}
		if (!IsAppInstalled(app, (AppRootDirDoesNotExist e) =>
		{
			throw new UnpredictedException(e);
		}, null))
		{
			throw new AppIsNotInstalled();
		}
		if (DoesAppNeedRepair(app, (AppRootDirDoesNotExist e) =>
		{
			throw new UnpredictedException(e);
		}, null))
		{
			throw new AppNeedsRepair();
		}
		AppStartInfo? appStartInfo = GetAppStartInfo(app, null);
		if (!appStartInfo.HasValue)
		{
			throw new AppNeedsRepair();
		}
		Path path = GetAppEntryFullPath(app, appStartInfo.Value.ExecutablePath);
		Path? v = GetParentPath(path);
		Text? executableArgs = appStartInfo.Value.ExecutableArgs;
		if (appProcessExtraArgsAsText.HasValue)
		{
			executableArgs = ((!executableArgs.HasValue) ? appProcessExtraArgsAsText : new Text?(new Text($"{executableArgs.Value} {appProcessExtraArgsAsText}")));
		}
		Assert.IsNotNull(v);
		return StartProcess(path, executableArgs, v.Value, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}, (NotExistingFile e) =>
		{
			throw new AppNeedsRepair();
		});
	}

	public StartApp(DoesAppRootDirExistDelegate doesAppRootDirExist, IsAppInstalledDelegate isAppInstalled, DoesAppNeedRepairDelegate doesAppNeedRepair, GetAppStartInfoDelegate getAppStartInfo, GetAppEntryFullPathDelegate getAppEntryFullPath, GetParentPathDelegate getParentPath, StartProcessDelegate startProcess, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(isAppInstalled);
		Assert.IsNotNull(doesAppNeedRepair);
		Assert.IsNotNull(getAppStartInfo);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(getParentPath);
		Assert.IsNotNull(startProcess);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppRootDirExist = doesAppRootDirExist;
		IsAppInstalled = isAppInstalled;
		DoesAppNeedRepair = doesAppNeedRepair;
		GetAppStartInfo = getAppStartInfo;
		GetAppEntryFullPath = getAppEntryFullPath;
		GetParentPath = getParentPath;
		StartProcess = startProcess;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ProcessId DelegateExecute(App app, Text? appProcessExtraArgsAsText, HandleExceptionAndReturnDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir, HandleExceptionAndReturnDelegate<AppIsNotInstalled> onAppIsNotInstalled, HandleExceptionAndReturnDelegate<AppNeedsRepair> onAppNeedsRepair)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(appProcessExtraArgsAsText, "appProcessExtraArgsAsText");
		LogInfo("Executing StartApp...");
		LogTrace("app = " + app);
		Text? text = appProcessExtraArgsAsText;
		LogTrace("appProcessExtraArgsAsText = " + (text.HasValue ? ((string)text.GetValueOrDefault()) : null));
		try
		{
			BeginLogRegion(enabled: true);
			ProcessId processId = Execute(app, appProcessExtraArgsAsText);
			EndLogRegion(enabled: true);
			ProcessId processId2 = processId;
			LogTrace("result = " + processId2.ToString());
			LogInfo("StartApp finished: success.");
			return processId;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartApp finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			object obj = onAppRootDirDoesNotExist(e);
			if (obj == null)
			{
				return default;
			}
			return (ProcessId)obj;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartApp finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			object obj2 = onUnauthorizedAccessToAppRootDir(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (ProcessId)obj2;
		}
		catch (AppIsNotInstalled e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartApp finished: AppIsNotInstalled.");
			if (onAppIsNotInstalled == null)
			{
				throw;
			}
			object obj3 = onAppIsNotInstalled(e3);
			if (obj3 == null)
			{
				return default;
			}
			return (ProcessId)obj3;
		}
		catch (AppNeedsRepair e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartApp finished: AppNeedsRepair.");
			if (onAppNeedsRepair == null)
			{
				throw;
			}
			object obj4 = onAppNeedsRepair(e4);
			if (obj4 == null)
			{
				return default;
			}
			return (ProcessId)obj4;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartApp has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("StartApp failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("StartApp finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public StartAppDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new StartApp(context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<IsAppInstalledDelegate>(), context.Resolve<DoesAppNeedRepairDelegate>(), context.Resolve<GetAppStartInfoDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<GetParentPathDelegate>(), context.Resolve<StartProcessDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
