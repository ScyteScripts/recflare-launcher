using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppLastUnfinishedWriteFileDataOperationInfo
{
	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public GetAppInfoDelegate GetAppInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppLastUnfinishedWriteFileDataOperationInfo? Execute(App app)
	{
		if (!DoesAppRootDirExist(app, null))
		{
			throw new AppRootDirDoesNotExist();
		}
		return GetAppInfo(app, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}).LastUnfinishedWriteFileDataOperationInfo;
	}

	public GetAppLastUnfinishedWriteFileDataOperationInfo(DoesAppRootDirExistDelegate doesAppRootDirExist, GetAppInfoDelegate getAppInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(getAppInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppRootDirExist = doesAppRootDirExist;
		GetAppInfo = getAppInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppLastUnfinishedWriteFileDataOperationInfo? DelegateExecute(App app, HandleExceptionAndReturnDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GetAppLastUnfinishedWriteFileDataOperationInfo...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			AppLastUnfinishedWriteFileDataOperationInfo? appLastUnfinishedWriteFileDataOperationInfo = Execute(app);
			EndLogRegion(enabled: true);
			AppLastUnfinishedWriteFileDataOperationInfo? appLastUnfinishedWriteFileDataOperationInfo2 = appLastUnfinishedWriteFileDataOperationInfo;
			LogTrace("result = " + appLastUnfinishedWriteFileDataOperationInfo2.ToString());
			LogInfo("GetAppLastUnfinishedWriteFileDataOperationInfo finished: success.");
			return appLastUnfinishedWriteFileDataOperationInfo;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppLastUnfinishedWriteFileDataOperationInfo finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			object obj = onAppRootDirDoesNotExist(e);
			if (obj == null)
			{
				return null;
			}
			return (AppLastUnfinishedWriteFileDataOperationInfo?)obj;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppLastUnfinishedWriteFileDataOperationInfo finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			object obj2 = onUnauthorizedAccessToAppRootDir(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (AppLastUnfinishedWriteFileDataOperationInfo?)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppLastUnfinishedWriteFileDataOperationInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppLastUnfinishedWriteFileDataOperationInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppLastUnfinishedWriteFileDataOperationInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppLastUnfinishedWriteFileDataOperationInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppLastUnfinishedWriteFileDataOperationInfo(context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<GetAppInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
