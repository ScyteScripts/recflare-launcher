using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppLicenseKey
{
	public GetAppInfoDelegate GetAppInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppLicenseKey? Execute(App app)
	{
		return GetAppInfo(app, null).LicenseKey;
	}

	public GetAppLicenseKey(GetAppInfoDelegate getAppInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppInfo = getAppInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppLicenseKey? DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GetAppLicenseKey...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			AppLicenseKey? appLicenseKey = Execute(app);
			EndLogRegion(enabled: true);
			AppLicenseKey? appLicenseKey2 = appLicenseKey;
			LogTrace("result = " + (appLicenseKey2.HasValue ? ((string)appLicenseKey2.GetValueOrDefault()) : null));
			LogInfo("GetAppLicenseKey finished: success.");
			return appLicenseKey;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppLicenseKey finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppLicenseKey?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppLicenseKey has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppLicenseKey failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppLicenseKey finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppLicenseKeyDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppLicenseKey(context.Resolve<GetAppInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
