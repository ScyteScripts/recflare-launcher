using System;
using System.Threading;
using Autofac;
using PatchKit.Api;
using PatchKit.Api.Models.Keys;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public class ValidateAppLicenseKeyA
{
	public IKeysApiConnection KeysApiConnection { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppLicenseSecret Execute(AppSecret appSecret, AppLicenseKey licenseKey, AppLicenseSecret? lastLicenseSecret, CancellationToken cancellationToken)
	{
		try
		{
			LicenseKey licenseKey2 = TempApiUtils.MakeApiCall((PatchKit.Core.Timeout timeout) => KeysApiConnection.GetKeyInfo(licenseKey, appSecret, lastLicenseSecret?.Value, timeout, cancellationToken));
			Assert.IsNotNull(licenseKey2.KeySecret);
			return new AppLicenseSecret(licenseKey2.KeySecret);
		}
		catch (ApiResponseException ex)
		{
			if (ex.StatusCode == 404)
			{
				throw new InvalidAppLicenseKey(licenseKey);
			}
			if (ex.StatusCode == 410)
			{
				throw new BlockedAppLicenseKey(licenseKey);
			}
			throw new UnpredictedException(ex);
		}
	}

	public ValidateAppLicenseKeyA(IKeysApiConnection keysApiConnection, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(keysApiConnection);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		KeysApiConnection = keysApiConnection;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppLicenseSecret DelegateExecute(AppSecret appSecret, AppLicenseKey licenseKey, AppLicenseSecret? lastLicenseSecret, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<BlockedAppLicenseKey> onBlockedAppLicenseKey, HandleExceptionAndReturnDelegate<InvalidAppLicenseKey> onInvalidAppLicenseKey)
	{
		Arg.IsValid(appSecret, "appSecret");
		Arg.IsValid(licenseKey, "licenseKey");
		Arg.IsValid(lastLicenseSecret, "lastLicenseSecret");
		LogInfo("Executing ValidateAppLicenseKeyA...");
		LogTrace("appSecret = " + appSecret);
		LogTrace("licenseKey = " + licenseKey);
		AppLicenseSecret? appLicenseSecret = lastLicenseSecret;
		LogTrace("lastLicenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			AppLicenseSecret appLicenseSecret2 = Execute(appSecret, licenseKey, lastLicenseSecret, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + appLicenseSecret2);
			LogInfo("ValidateAppLicenseKeyA finished: success.");
			return appLicenseSecret2;
		}
		catch (BlockedAppLicenseKey e)
		{
			EndLogRegion(enabled: true);
			LogInfo("ValidateAppLicenseKeyA finished: BlockedAppLicenseKey.");
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
			LogInfo("ValidateAppLicenseKeyA finished: InvalidAppLicenseKey.");
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
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("ValidateAppLicenseKeyA has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("ValidateAppLicenseKeyA failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("ValidateAppLicenseKeyA finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ValidateAppLicenseKeyADelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ValidateAppLicenseKeyA(context.Resolve<IKeysApiConnection>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
