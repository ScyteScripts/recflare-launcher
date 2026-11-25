using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemoteDiffInfo
{
	public GetAppRemoteDiffInfoDelegate GetAppRemoteDiffInfo { get; }

	public GetFilteredNewAppRemotePackageInfoBDelegate GetFilteredNewAppRemotePackageInfoB { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageInfo Execute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, ImmutableArray<AppPath> dataFiles, ImmutableArray<AppPath> patchFiles, CancellationToken cancellationToken)
	{
		AppRemotePackageInfo info = GetAppRemoteDiffInfo(appRemote, versionPatchLinkId, licenseSecret, password, cancellationToken, null, null, null, null);
		return GetFilteredNewAppRemotePackageInfoB(info, dataFiles, patchFiles);
	}

	public GetFilteredNewAppRemoteDiffInfo(GetAppRemoteDiffInfoDelegate getAppRemoteDiffInfo, GetFilteredNewAppRemotePackageInfoBDelegate getFilteredNewAppRemotePackageInfoB, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteDiffInfo);
		Assert.IsNotNull(getFilteredNewAppRemotePackageInfoB);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteDiffInfo = getAppRemoteDiffInfo;
		GetFilteredNewAppRemotePackageInfoB = getFilteredNewAppRemotePackageInfoB;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageInfo DelegateExecute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, ImmutableArray<AppPath> dataFiles, ImmutableArray<AppPath> patchFiles, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionPatchLinkNotFound> onAppRemoteVersionPatchLinkNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionPatchLinkId, "versionPatchLinkId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		Arg.IsValid(dataFiles, "dataFiles");
		Arg.IsValid(patchFiles, "patchFiles");
		LogInfo("Executing GetFilteredNewAppRemoteDiffInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionPatchLinkId appVersionPatchLinkId = versionPatchLinkId;
		LogTrace("versionPatchLinkId = " + appVersionPatchLinkId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		ImmutableArray<AppPath> immutableArray = dataFiles;
		LogTrace("dataFiles = " + immutableArray.ToString());
		immutableArray = patchFiles;
		LogTrace("patchFiles = " + immutableArray.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo = Execute(appRemote, versionPatchLinkId, licenseSecret, password, dataFiles, patchFiles, cancellationToken);
			EndLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo2 = appRemotePackageInfo;
			LogTrace("result = " + appRemotePackageInfo2.ToString());
			LogInfo("GetFilteredNewAppRemoteDiffInfo finished: success.");
			return appRemotePackageInfo;
		}
		catch (AppRemoteVersionPatchLinkNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteDiffInfo finished: AppRemoteVersionPatchLinkNotFound.");
			if (onAppRemoteVersionPatchLinkNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionPatchLinkNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (AppRemotePackageInfo)obj;
		}
		catch (InvalidAppPassword e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteDiffInfo finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj2 = onInvalidAppPassword(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (AppRemotePackageInfo)obj2;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteDiffInfo finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj3 = onInvalidAppAuthKey(e3);
			if (obj3 == null)
			{
				return default;
			}
			return (AppRemotePackageInfo)obj3;
		}
		catch (ExpiredAppAuthKey e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteDiffInfo finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj4 = onExpiredAppAuthKey(e4);
			if (obj4 == null)
			{
				return default;
			}
			return (AppRemotePackageInfo)obj4;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteDiffInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemoteDiffInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemoteDiffInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemoteDiffInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemoteDiffInfo(context.Resolve<GetAppRemoteDiffInfoDelegate>(), context.Resolve<GetFilteredNewAppRemotePackageInfoBDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
