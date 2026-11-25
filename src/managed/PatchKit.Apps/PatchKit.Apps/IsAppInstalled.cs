using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class IsAppInstalled
{
	public GetAppInstalledVersionIdDelegate GetAppInstalledVersionId { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(App app)
	{
		return GetAppInstalledVersionId(app, null, null).HasValue;
	}

	public IsAppInstalled(GetAppInstalledVersionIdDelegate getAppInstalledVersionId, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppInstalledVersionId);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppInstalledVersionId = getAppInstalledVersionId;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(App app, HandleExceptionAndReturnDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing IsAppInstalled...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(app);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsAppInstalled finished: success.");
			return result;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppInstalled finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			object obj = onAppRootDirDoesNotExist(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppInstalled finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			object obj2 = onUnauthorizedAccessToAppRootDir(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppInstalled has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsAppInstalled failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsAppInstalled finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsAppInstalledDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsAppInstalled(context.Resolve<GetAppInstalledVersionIdDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
