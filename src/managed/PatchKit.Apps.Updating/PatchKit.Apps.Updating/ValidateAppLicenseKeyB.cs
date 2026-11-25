using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class ValidateAppLicenseKeyB
{
	public ValidateAppLicenseKeyADelegate ValidateAppLicenseKeyA { get; }

	public GetAppLicenseSecretDelegate GetAppLicenseSecret { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppLicenseSecret Execute(App app, AppSecret appSecret, AppLicenseKey licenseKey, CancellationToken cancellationToken)
	{
		AppLicenseSecret? lastLicenseSecret = GetAppLicenseSecret(app, licenseKey, null);
		return ValidateAppLicenseKeyA(appSecret, licenseKey, lastLicenseSecret, cancellationToken, null, null);
	}

	public ValidateAppLicenseKeyB(ValidateAppLicenseKeyADelegate validateAppLicenseKeyA, GetAppLicenseSecretDelegate getAppLicenseSecret, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(validateAppLicenseKeyA);
		Assert.IsNotNull(getAppLicenseSecret);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ValidateAppLicenseKeyA = validateAppLicenseKeyA;
		GetAppLicenseSecret = getAppLicenseSecret;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppLicenseSecret DelegateExecute(App app, AppSecret appSecret, AppLicenseKey licenseKey, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<BlockedAppLicenseKey> onBlockedAppLicenseKey, HandleExceptionAndReturnDelegate<InvalidAppLicenseKey> onInvalidAppLicenseKey, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(appSecret, "appSecret");
		Arg.IsValid(licenseKey, "licenseKey");
		LogInfo("Executing ValidateAppLicenseKeyB...");
		LogTrace("app = " + app);
		LogTrace("appSecret = " + appSecret);
		LogTrace("licenseKey = " + licenseKey);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			AppLicenseSecret appLicenseSecret = Execute(app, appSecret, licenseKey, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + appLicenseSecret);
			LogInfo("ValidateAppLicenseKeyB finished: success.");
			return appLicenseSecret;
		}
		catch (BlockedAppLicenseKey e)
		{
			EndLogRegion(enabled: true);
			LogInfo("ValidateAppLicenseKeyB finished: BlockedAppLicenseKey.");
			if (onBlockedAppLicenseKey == null)
			{
				throw;
			}
			object obj = onBlockedAppLicenseKey(e);
			if (obj == null)
			{
				return default;
			}
			return (AppLicenseSecret)obj;
		}
		catch (InvalidAppLicenseKey e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("ValidateAppLicenseKeyB finished: InvalidAppLicenseKey.");
			if (onInvalidAppLicenseKey == null)
			{
				throw;
			}
			object obj2 = onInvalidAppLicenseKey(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (AppLicenseSecret)obj2;
		}
		catch (UnauthorizedAccess e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("ValidateAppLicenseKeyB finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj3 = onUnauthorizedAccess(e3);
			if (obj3 == null)
			{
				return default;
			}
			return (AppLicenseSecret)obj3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("ValidateAppLicenseKeyB has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("ValidateAppLicenseKeyB failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("ValidateAppLicenseKeyB finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ValidateAppLicenseKeyBDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ValidateAppLicenseKeyB(context.Resolve<ValidateAppLicenseKeyADelegate>(), context.Resolve<GetAppLicenseSecretDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
