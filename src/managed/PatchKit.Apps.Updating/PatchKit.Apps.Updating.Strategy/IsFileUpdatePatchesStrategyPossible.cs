using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Strategy;

public class IsFileUpdatePatchesStrategyPossible
{
	public IsUpdateFileConsistentDelegate IsUpdateFileConsistent { get; }

	public GetAppRemoteContentInfoDelegate GetAppRemoteContentInfo { get; }

	public GetAppRemoteDiffInfoDelegate GetAppRemoteDiffInfo { get; }

	public GetAppRemoteLowestVersionWithDiffIdDelegate GetAppRemoteLowestVersionWithDiffId { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(AppRemote appRemote, AppVersionId targetVersionId, AppLicenseSecret? licenseSecret, AppPassword? password, UpdateFileState state, CancellationToken cancellationToken)
	{
		if ((int)targetVersionId < (int)state.VersionId)
		{
			return false;
		}
		if (!IsUpdateFileConsistent(appRemote, state, cancellationToken, null))
		{
			return false;
		}
		AppRemotePack1UnpackInfo? pack1Info = GetAppRemoteContentInfo(appRemote, targetVersionId, licenseSecret, password, cancellationToken, null, null, null, null).UnpackInfo.Pack1Info;
		BytesCount? bytesCount = (pack1Info.HasValue ? pack1Info.GetValueOrDefault().FilesInfo.FirstOrNull((AppRemotePack1FileUnpackInfo x) => (string)x.Path == (string)state.Path) : ((AppRemotePack1FileUnpackInfo?)null))?.Size;
		AppVersionPatchLinkId versionPatchLinkId = new AppVersionPatchLinkId(state.VersionId, targetVersionId);
		AppRemotePackageInfo? appRemotePackageInfo;
		try
		{
			appRemotePackageInfo = GetAppRemoteDiffInfo(appRemote, versionPatchLinkId, licenseSecret, password, cancellationToken, null, null, null, null);
		}
		catch (AppRemoteVersionPatchLinkNotFound)
		{
			appRemotePackageInfo = null;
		}
		AppRemotePackagePatchFileInfo? directPatchDiffPatchFileInfo = ((!appRemotePackageInfo.HasValue) ? ((AppRemotePackagePatchFileInfo?)null) : appRemotePackageInfo.GetValueOrDefault().FilesInfo.FirstOrNull((AppRemotePackageFileInfo x) => x.PatchFileInfo.HasValue && (string)x.PatchFileInfo.Value.Path == (string)state.Path)?.PatchFileInfo);
		if (directPatchDiffPatchFileInfo.HasValue)
		{
			AppRemotePack1FileUnpackInfo? appRemotePack1FileUnpackInfo;
			if (!appRemotePackageInfo.HasValue)
			{
				appRemotePack1FileUnpackInfo = null;
			}
			else
			{
				pack1Info = appRemotePackageInfo.GetValueOrDefault().UnpackInfo.Pack1Info;
				appRemotePack1FileUnpackInfo = (pack1Info.HasValue ? pack1Info.GetValueOrDefault().FilesInfo.FirstOrNull((AppRemotePack1FileUnpackInfo x) => (string)x.Path == (string)directPatchDiffPatchFileInfo.Value.Path) : ((AppRemotePack1FileUnpackInfo?)null));
			}
			BytesCount? bytesCount2 = appRemotePack1FileUnpackInfo?.Size;
			if (bytesCount.HasValue && bytesCount2.HasValue)
			{
				return (long)bytesCount2.Value < (long)bytesCount.Value;
			}
			return true;
		}
		AppVersionId? appVersionId = GetAppRemoteLowestVersionWithDiffId(appRemote, cancellationToken, null);
		if (!appVersionId.HasValue)
		{
			return false;
		}
		if ((int)appVersionId.Value > (int)state.VersionId + 1)
		{
			return false;
		}
		BytesCount? bytesCount3 = new BytesCount(0L);
		AppVersionId appVersionId2 = new AppVersionId((int)state.VersionId + 1);
		while ((int)appVersionId2 <= (int)targetVersionId)
		{
			AppRemotePackageInfo? appRemotePackageInfo2;
			try
			{
				appRemotePackageInfo2 = GetAppRemoteDiffInfo(appRemote, new AppVersionPatchLinkId(new AppVersionId((int)appVersionId2 - 1), appVersionId2), licenseSecret, password, cancellationToken, null, null, null, null);
			}
			catch (AppRemoteVersionPatchLinkNotFound)
			{
				appRemotePackageInfo2 = null;
			}
			if (!((!appRemotePackageInfo2.HasValue) ? ((AppRemotePackagePatchFileInfo?)null) : appRemotePackageInfo2.GetValueOrDefault().FilesInfo.FirstOrNull((AppRemotePackageFileInfo x) => x.PatchFileInfo.HasValue && (string)x.PatchFileInfo.Value.Path == (string)state.Path)?.PatchFileInfo).HasValue)
			{
				return false;
			}
			AppRemotePack1FileUnpackInfo? appRemotePack1FileUnpackInfo2;
			if (!appRemotePackageInfo2.HasValue)
			{
				appRemotePack1FileUnpackInfo2 = null;
			}
			else
			{
				pack1Info = appRemotePackageInfo2.GetValueOrDefault().UnpackInfo.Pack1Info;
				appRemotePack1FileUnpackInfo2 = (pack1Info.HasValue ? pack1Info.GetValueOrDefault().FilesInfo.FirstOrNull((AppRemotePack1FileUnpackInfo x) => (string)x.Path == (string)state.Path) : ((AppRemotePack1FileUnpackInfo?)null));
			}
			AppRemotePack1FileUnpackInfo? appRemotePack1FileUnpackInfo3 = appRemotePack1FileUnpackInfo2;
			bytesCount3 = ((!appRemotePack1FileUnpackInfo3.HasValue || !bytesCount3.HasValue) ? ((BytesCount?)null) : new BytesCount?(new BytesCount(bytesCount3.Value + appRemotePack1FileUnpackInfo3.Value.Size)));
			appVersionId2 = new AppVersionId((int)appVersionId2 + 1);
		}
		if (bytesCount.HasValue && bytesCount3.HasValue)
		{
			return (long)bytesCount3.Value < (long)bytesCount.Value;
		}
		return true;
	}

	public IsFileUpdatePatchesStrategyPossible(IsUpdateFileConsistentDelegate isUpdateFileConsistent, GetAppRemoteContentInfoDelegate getAppRemoteContentInfo, GetAppRemoteDiffInfoDelegate getAppRemoteDiffInfo, GetAppRemoteLowestVersionWithDiffIdDelegate getAppRemoteLowestVersionWithDiffId, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(isUpdateFileConsistent);
		Assert.IsNotNull(getAppRemoteContentInfo);
		Assert.IsNotNull(getAppRemoteDiffInfo);
		Assert.IsNotNull(getAppRemoteLowestVersionWithDiffId);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		IsUpdateFileConsistent = isUpdateFileConsistent;
		GetAppRemoteContentInfo = getAppRemoteContentInfo;
		GetAppRemoteDiffInfo = getAppRemoteDiffInfo;
		GetAppRemoteLowestVersionWithDiffId = getAppRemoteLowestVersionWithDiffId;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(AppRemote appRemote, AppVersionId targetVersionId, AppLicenseSecret? licenseSecret, AppPassword? password, UpdateFileState state, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<AppRemoteVersionPatchLinkNotFound> onAppRemoteVersionPatchLinkNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(targetVersionId, "targetVersionId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		Arg.IsValid(state, "state");
		LogInfo("Executing IsFileUpdatePatchesStrategyPossible...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = targetVersionId;
		LogTrace("targetVersionId = " + appVersionId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		UpdateFileState updateFileState = state;
		LogTrace("state = " + updateFileState.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(appRemote, targetVersionId, licenseSecret, password, state, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsFileUpdatePatchesStrategyPossible finished: success.");
			return result;
		}
		catch (AppRemoteNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileUpdatePatchesStrategyPossible finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteNotFound(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (AppRemoteVersionNotFound e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileUpdatePatchesStrategyPossible finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj2 = onAppRemoteVersionNotFound(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (AppRemoteVersionPatchLinkNotFound e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileUpdatePatchesStrategyPossible finished: AppRemoteVersionPatchLinkNotFound.");
			if (onAppRemoteVersionPatchLinkNotFound == null)
			{
				throw;
			}
			object obj3 = onAppRemoteVersionPatchLinkNotFound(e3);
			if (obj3 == null)
			{
				return false;
			}
			return (bool)obj3;
		}
		catch (InvalidAppPassword e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileUpdatePatchesStrategyPossible finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj4 = onInvalidAppPassword(e4);
			if (obj4 == null)
			{
				return false;
			}
			return (bool)obj4;
		}
		catch (InvalidAppAuthKey e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileUpdatePatchesStrategyPossible finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj5 = onInvalidAppAuthKey(e5);
			if (obj5 == null)
			{
				return false;
			}
			return (bool)obj5;
		}
		catch (ExpiredAppAuthKey e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileUpdatePatchesStrategyPossible finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj6 = onExpiredAppAuthKey(e6);
			if (obj6 == null)
			{
				return false;
			}
			return (bool)obj6;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileUpdatePatchesStrategyPossible has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileUpdatePatchesStrategyPossible failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileUpdatePatchesStrategyPossible finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsFileUpdatePatchesStrategyPossibleDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsFileUpdatePatchesStrategyPossible(context.Resolve<IsUpdateFileConsistentDelegate>(), context.Resolve<GetAppRemoteContentInfoDelegate>(), context.Resolve<GetAppRemoteDiffInfoDelegate>(), context.Resolve<GetAppRemoteLowestVersionWithDiffIdDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
