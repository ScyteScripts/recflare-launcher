using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class SetAppLicenseKeyAndSecret
{
	public EncryptAppLicenseSecretDelegate EncryptAppLicenseSecret { get; }

	public GetAppInfoDelegate GetAppInfo { get; }

	public SetAppInfoDelegate SetAppInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppLicenseKey licenseKey, AppLicenseSecret licenseSecret)
	{
		AppEncryptedLicenseSecret value = EncryptAppLicenseSecret(licenseSecret, licenseKey);
		AppInfo value2 = GetAppInfo(app, null).ModifyLicenseKey(licenseKey).ModifyEncryptedLicenseSecret(value);
		SetAppInfo(app, value2, null);
	}

	public SetAppLicenseKeyAndSecret(EncryptAppLicenseSecretDelegate encryptAppLicenseSecret, GetAppInfoDelegate getAppInfo, SetAppInfoDelegate setAppInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(encryptAppLicenseSecret);
		Assert.IsNotNull(getAppInfo);
		Assert.IsNotNull(setAppInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		EncryptAppLicenseSecret = encryptAppLicenseSecret;
		GetAppInfo = getAppInfo;
		SetAppInfo = setAppInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppLicenseKey licenseKey, AppLicenseSecret licenseSecret, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(licenseKey, "licenseKey");
		Arg.IsValid(licenseSecret, "licenseSecret");
		LogInfo("Executing SetAppLicenseKeyAndSecret...");
		LogTrace("app = " + app);
		LogTrace("licenseKey = " + licenseKey);
		LogTrace("licenseSecret = " + licenseSecret);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, licenseKey, licenseSecret);
			EndLogRegion(enabled: true);
			LogInfo("SetAppLicenseKeyAndSecret finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppLicenseKeyAndSecret finished: UnauthorizedAccess.");
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
			LogInfo("SetAppLicenseKeyAndSecret has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppLicenseKeyAndSecret failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppLicenseKeyAndSecret finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SetAppLicenseKeyAndSecretDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SetAppLicenseKeyAndSecret(context.Resolve<EncryptAppLicenseSecretDelegate>(), context.Resolve<GetAppInfoDelegate>(), context.Resolve<SetAppInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
