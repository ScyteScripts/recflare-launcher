using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteDiffPatchFileInfo
{
	public GetAppRemoteDiffInfoDelegate GetAppRemoteDiffInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackagePatchFileInfo? Execute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, AppPath path, CancellationToken cancellationToken)
	{
		return GetAppRemoteDiffInfo(appRemote, versionPatchLinkId, licenseSecret, password, cancellationToken, null, null, null, null).FilesInfo.FirstOrNull((AppRemotePackageFileInfo x) => x.PatchFileInfo.HasValue && (string)x.PatchFileInfo.Value.Path == (string)path)?.PatchFileInfo;
	}

	public GetAppRemoteDiffPatchFileInfo(GetAppRemoteDiffInfoDelegate getAppRemoteDiffInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteDiffInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteDiffInfo = getAppRemoteDiffInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackagePatchFileInfo? DelegateExecute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, AppPath path, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionPatchLinkNotFound> onAppRemoteVersionPatchLinkNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionPatchLinkId, "versionPatchLinkId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetAppRemoteDiffPatchFileInfo...");
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
			AppRemotePackagePatchFileInfo? appRemotePackagePatchFileInfo = Execute(appRemote, versionPatchLinkId, licenseSecret, password, path, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemotePackagePatchFileInfo? appRemotePackagePatchFileInfo2 = appRemotePackagePatchFileInfo;
			LogTrace("result = " + appRemotePackagePatchFileInfo2.ToString());
			LogInfo("GetAppRemoteDiffPatchFileInfo finished: success.");
			return appRemotePackagePatchFileInfo;
		}
		catch (AppRemoteVersionPatchLinkNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffPatchFileInfo finished: AppRemoteVersionPatchLinkNotFound.");
			if (onAppRemoteVersionPatchLinkNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionPatchLinkNotFound(e);
			if (obj == null)
			{
				return null;
			}
			return (AppRemotePackagePatchFileInfo?)obj;
		}
		catch (InvalidAppPassword e2)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffPatchFileInfo finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj2 = onInvalidAppPassword(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (AppRemotePackagePatchFileInfo?)obj2;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffPatchFileInfo finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj3 = onInvalidAppAuthKey(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (AppRemotePackagePatchFileInfo?)obj3;
		}
		catch (ExpiredAppAuthKey e4)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffPatchFileInfo finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj4 = onExpiredAppAuthKey(e4);
			if (obj4 == null)
			{
				return null;
			}
			return (AppRemotePackagePatchFileInfo?)obj4;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffPatchFileInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteDiffPatchFileInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteDiffPatchFileInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteDiffPatchFileInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteDiffPatchFileInfo(context.Resolve<GetAppRemoteDiffInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
