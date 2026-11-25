using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class SetAppLastInstalledVersionId
{
	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public GetAppInfoDelegate GetAppInfo { get; }

	public SetAppInfoDelegate SetAppInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppVersionId? value)
	{
		if (!DoesAppRootDirExist(app, null))
		{
			throw new AppRootDirDoesNotExist();
		}
		AppInfo value2 = GetAppInfo(app, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}).ModifyLastInstalledVersionId(value);
		SetAppInfo(app, value2, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
	}

	public SetAppLastInstalledVersionId(DoesAppRootDirExistDelegate doesAppRootDirExist, GetAppInfoDelegate getAppInfo, SetAppInfoDelegate setAppInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(getAppInfo);
		Assert.IsNotNull(setAppInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppRootDirExist = doesAppRootDirExist;
		GetAppInfo = getAppInfo;
		SetAppInfo = setAppInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppVersionId? value, HandleExceptionDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(value, "value");
		LogInfo("Executing SetAppLastInstalledVersionId...");
		LogTrace("app = " + app);
		AppVersionId? appVersionId = value;
		LogTrace("value = " + appVersionId.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, value);
			EndLogRegion(enabled: true);
			LogInfo("SetAppLastInstalledVersionId finished: success.");
			return null;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppLastInstalledVersionId finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			onAppRootDirDoesNotExist(e);
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppLastInstalledVersionId finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			onUnauthorizedAccessToAppRootDir(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppLastInstalledVersionId has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppLastInstalledVersionId failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppLastInstalledVersionId finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SetAppLastInstalledVersionIdDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SetAppLastInstalledVersionId(context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<GetAppInfoDelegate>(), context.Resolve<SetAppInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
