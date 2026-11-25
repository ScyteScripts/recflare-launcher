using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppLicenseSecret
{
	public GetAppEncryptedLicenseSecretDelegate GetAppEncryptedLicenseSecret { get; }

	public DecryptAppLicenseSecretDelegate DecryptAppLicenseSecret { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppLicenseSecret? Execute(App app, AppLicenseKey licenseKey)
	{
		AppEncryptedLicenseSecret? appEncryptedLicenseSecret = GetAppEncryptedLicenseSecret(app, null);
		if (!appEncryptedLicenseSecret.HasValue)
		{
			return null;
		}
		return DecryptAppLicenseSecret(appEncryptedLicenseSecret.Value, licenseKey);
	}

	public GetAppLicenseSecret(GetAppEncryptedLicenseSecretDelegate getAppEncryptedLicenseSecret, DecryptAppLicenseSecretDelegate decryptAppLicenseSecret, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEncryptedLicenseSecret);
		Assert.IsNotNull(decryptAppLicenseSecret);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEncryptedLicenseSecret = getAppEncryptedLicenseSecret;
		DecryptAppLicenseSecret = decryptAppLicenseSecret;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppLicenseSecret? DelegateExecute(App app, AppLicenseKey licenseKey, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(licenseKey, "licenseKey");
		LogInfo("Executing GetAppLicenseSecret...");
		LogTrace("app = " + app);
		LogTrace("licenseKey = " + licenseKey);
		try
		{
			BeginLogRegion(enabled: true);
			AppLicenseSecret? appLicenseSecret = Execute(app, licenseKey);
			EndLogRegion(enabled: true);
			AppLicenseSecret? appLicenseSecret2 = appLicenseSecret;
			LogTrace("result = " + (appLicenseSecret2.HasValue ? ((string)appLicenseSecret2.GetValueOrDefault()) : null));
			LogInfo("GetAppLicenseSecret finished: success.");
			return appLicenseSecret;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppLicenseSecret finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppLicenseSecret?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppLicenseSecret has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppLicenseSecret failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppLicenseSecret finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppLicenseSecretDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppLicenseSecret(context.Resolve<GetAppEncryptedLicenseSecretDelegate>(), context.Resolve<DecryptAppLicenseSecretDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
