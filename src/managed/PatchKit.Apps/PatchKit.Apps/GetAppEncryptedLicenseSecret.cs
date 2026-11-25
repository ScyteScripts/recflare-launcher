using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppEncryptedLicenseSecret
{
	public GetAppInfoDelegate GetAppInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppEncryptedLicenseSecret? Execute(App app)
	{
		return GetAppInfo(app, null).EncryptedLicenseSecret;
	}

	public GetAppEncryptedLicenseSecret(GetAppInfoDelegate getAppInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

	private AppEncryptedLicenseSecret? DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GetAppEncryptedLicenseSecret...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			AppEncryptedLicenseSecret? appEncryptedLicenseSecret = Execute(app);
			EndLogRegion(enabled: true);
			AppEncryptedLicenseSecret? appEncryptedLicenseSecret2 = appEncryptedLicenseSecret;
			LogTrace("result = " + (appEncryptedLicenseSecret2.HasValue ? ((string)appEncryptedLicenseSecret2.GetValueOrDefault()) : null));
			LogInfo("GetAppEncryptedLicenseSecret finished: success.");
			return appEncryptedLicenseSecret;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppEncryptedLicenseSecret finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppEncryptedLicenseSecret?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppEncryptedLicenseSecret has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppEncryptedLicenseSecret failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppEncryptedLicenseSecret finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppEncryptedLicenseSecretDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppEncryptedLicenseSecret(context.Resolve<GetAppInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
