using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class DoesAppRemoteDiffContainsPatchFile
{
	public GetAppRemoteDiffPatchFileInfoDelegate GetAppRemoteDiffPatchFileInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, AppPath path, CancellationToken cancellationToken)
	{
		return GetAppRemoteDiffPatchFileInfo(appRemote, versionPatchLinkId, licenseSecret, password, path, cancellationToken, null, null, null, null).HasValue;
	}

	public DoesAppRemoteDiffContainsPatchFile(GetAppRemoteDiffPatchFileInfoDelegate getAppRemoteDiffPatchFileInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteDiffPatchFileInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteDiffPatchFileInfo = getAppRemoteDiffPatchFileInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, AppPath path, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionPatchLinkNotFound> onAppRemoteVersionPatchLinkNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionPatchLinkId, "versionPatchLinkId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		Arg.IsValid(path, "path");
		LogInfo("Executing DoesAppRemoteDiffContainsPatchFile...");
		LogTrace("appRemote = " + appRemote);
		AppVersionPatchLinkId appVersionPatchLinkId = versionPatchLinkId;
		LogTrace("versionPatchLinkId = " + appVersionPatchLinkId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		LogTrace("path = " + path);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			bool result = Execute(appRemote, versionPatchLinkId, licenseSecret, password, path, cancellationToken);
			EndLogRegion(enabled: false);
			LogTrace("result = " + result);
			LogInfo("DoesAppRemoteDiffContainsPatchFile finished: success.");
			return result;
		}
		catch (AppRemoteVersionPatchLinkNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("DoesAppRemoteDiffContainsPatchFile finished: AppRemoteVersionPatchLinkNotFound.");
			if (onAppRemoteVersionPatchLinkNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionPatchLinkNotFound(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (InvalidAppPassword e2)
		{
			EndLogRegion(enabled: false);
			LogInfo("DoesAppRemoteDiffContainsPatchFile finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj2 = onInvalidAppPassword(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: false);
			LogInfo("DoesAppRemoteDiffContainsPatchFile finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj3 = onInvalidAppAuthKey(e3);
			if (obj3 == null)
			{
				return false;
			}
			return (bool)obj3;
		}
		catch (ExpiredAppAuthKey e4)
		{
			EndLogRegion(enabled: false);
			LogInfo("DoesAppRemoteDiffContainsPatchFile finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj4 = onExpiredAppAuthKey(e4);
			if (obj4 == null)
			{
				return false;
			}
			return (bool)obj4;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("DoesAppRemoteDiffContainsPatchFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("DoesAppRemoteDiffContainsPatchFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("DoesAppRemoteDiffContainsPatchFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DoesAppRemoteDiffContainsPatchFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DoesAppRemoteDiffContainsPatchFile(context.Resolve<GetAppRemoteDiffPatchFileInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
