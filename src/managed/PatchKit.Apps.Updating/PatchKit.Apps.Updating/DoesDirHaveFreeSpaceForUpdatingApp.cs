using System;
using System.Linq;
using System.Threading;
using Autofac;
using PatchKit.Api;
using PatchKit.Apps.Remote;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Apps.Updating.Strategy;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class DoesDirHaveFreeSpaceForUpdatingApp
{
	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public GetAppRemoteLatestVersionIdDelegate GetAppRemoteLatestVersionId { get; }

	public IsAppRemoteLicenseSecretRequiredDelegate IsAppRemoteLicenseSecretRequired { get; }

	public IsAppRemoteAuthTokenRequiredDelegate IsAppRemoteAuthTokenRequired { get; }

	public IsAppRemotePasswordRequiredDelegate IsAppRemotePasswordRequired { get; }

	public GetAppLicenseKeyDelegate GetAppLicenseKey { get; }

	public GetAppPasswordDelegate GetAppPassword { get; }

	public ValidateAppLicenseKeyBDelegate ValidateAppLicenseKeyB { get; }

	public GetUpdateStrategyDelegate GetUpdateStrategy { get; }

	public GetFileUpdateStrategiesPackagesInfoDelegate GetFileUpdateStrategiesPackagesInfo { get; }

	public GetDirFreeSpaceBytesCountDelegate GetDirFreeSpaceBytesCount { get; }

	public GetPackageDelegate GetPackage { get; }

	public IApiConnection ApiConnection { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(App app, AppRemote appRemote, AppLicenseKey? appLicenseKey, AppPassword? appPassword, AppValidationMode appValidationMode, CancellationToken cancellationToken)
	{
		AppVersionId targetVersionId = GetAppRemoteLatestVersionId(appRemote, cancellationToken, null);
		bool flag = IsAppRemoteLicenseSecretRequired(appRemote, cancellationToken, null);
		bool flag2 = IsAppRemoteAuthTokenRequired(appRemote, cancellationToken, null);
		bool flag3 = IsAppRemotePasswordRequired(appRemote, cancellationToken, null);
		AppLicenseSecret? licenseSecret = null;
		if (flag && (!flag2 || !appRemote.AuthKey.HasValue))
		{
			if (!appLicenseKey.HasValue)
			{
				appLicenseKey = GetAppLicenseKey(app, (UnauthorizedAccess e) =>
				{
					throw new UnauthorizedAccessToAppRootDir();
				});
			}
			if (!appLicenseKey.HasValue)
			{
				throw new AppLicenseKeyRequired();
			}
			licenseSecret = ValidateAppLicenseKeyB(app, appRemote.Secret, appLicenseKey.Value, cancellationToken, null, null, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppRootDir();
			});
			if (flag2)
			{
				appRemote.AuthKey = new AppAuthKey(TempApiUtils.MakeApiCall((PatchKit.Core.Timeout timeout) =>
				{
					IApiConnection apiConnection = ApiConnection;
					string appSecret = appRemote.Secret;
					AppLicenseSecret? appLicenseSecret = licenseSecret;
					return apiConnection.AuthenticateAppWithKey(appSecret, appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null, timeout, cancellationToken);
				}).Jwt);
				licenseSecret = null;
			}
		}
		if (flag2 && !appRemote.AuthKey.HasValue)
		{
			throw new InvalidAppAuthKey();
		}
		if (flag3)
		{
			appPassword = appPassword ?? GetAppPassword(app, appRemote.Secret, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppRootDir();
			});
			if (!appPassword.HasValue)
			{
				throw new InvalidAppPassword();
			}
		}
		UpdateAppContext context = new UpdateAppContext(app, appRemote, targetVersionId, licenseSecret, appPassword);
		UpdateStrategy updateStrategy = GetUpdateStrategy(context, appValidationMode, cancellationToken, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}, null, null, null, null, null);
		ImmutableArray<AppRemotePackageInfo> immutableArray = GetFileUpdateStrategiesPackagesInfo(context.AppRemote, licenseSecret: context.LicenseSecret, password: context.Password, targetVersionId: context.TargetVersionId, strategies: updateStrategy.FileStrategies, cancellationToken: cancellationToken, onAppRemoteVersionNotFound: null, onInvalidAppPassword: null, onInvalidAppAuthKey: null, onExpiredAppAuthKey: null);
		BytesCount bytesCount = new BytesCount(immutableArray.Sum((AppRemotePackageInfo x) => x.Size));
		if (GetDirFreeSpaceBytesCount(context.App.Path, null).Value < bytesCount.Value)
		{
			return false;
		}
		return true;
	}

	public DoesDirHaveFreeSpaceForUpdatingApp(DoesAppRootDirExistDelegate doesAppRootDirExist, GetAppRemoteLatestVersionIdDelegate getAppRemoteLatestVersionId, IsAppRemoteLicenseSecretRequiredDelegate isAppRemoteLicenseSecretRequired, IsAppRemoteAuthTokenRequiredDelegate isAppRemoteAuthTokenRequired, IsAppRemotePasswordRequiredDelegate isAppRemotePasswordRequired, GetAppLicenseKeyDelegate getAppLicenseKey, GetAppPasswordDelegate getAppPassword, ValidateAppLicenseKeyBDelegate validateAppLicenseKeyB, GetUpdateStrategyDelegate getUpdateStrategy, GetFileUpdateStrategiesPackagesInfoDelegate getFileUpdateStrategiesPackagesInfo, GetDirFreeSpaceBytesCountDelegate getDirFreeSpaceBytesCount, GetPackageDelegate getPackage, IApiConnection apiConnection, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(getAppRemoteLatestVersionId);
		Assert.IsNotNull(isAppRemoteLicenseSecretRequired);
		Assert.IsNotNull(isAppRemoteAuthTokenRequired);
		Assert.IsNotNull(isAppRemotePasswordRequired);
		Assert.IsNotNull(getAppLicenseKey);
		Assert.IsNotNull(getAppPassword);
		Assert.IsNotNull(validateAppLicenseKeyB);
		Assert.IsNotNull(getUpdateStrategy);
		Assert.IsNotNull(getFileUpdateStrategiesPackagesInfo);
		Assert.IsNotNull(getDirFreeSpaceBytesCount);
		Assert.IsNotNull(getPackage);
		Assert.IsNotNull(apiConnection);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppRootDirExist = doesAppRootDirExist;
		GetAppRemoteLatestVersionId = getAppRemoteLatestVersionId;
		IsAppRemoteLicenseSecretRequired = isAppRemoteLicenseSecretRequired;
		IsAppRemoteAuthTokenRequired = isAppRemoteAuthTokenRequired;
		IsAppRemotePasswordRequired = isAppRemotePasswordRequired;
		GetAppLicenseKey = getAppLicenseKey;
		GetAppPassword = getAppPassword;
		ValidateAppLicenseKeyB = validateAppLicenseKeyB;
		GetUpdateStrategy = getUpdateStrategy;
		GetFileUpdateStrategiesPackagesInfo = getFileUpdateStrategiesPackagesInfo;
		GetDirFreeSpaceBytesCount = getDirFreeSpaceBytesCount;
		GetPackage = getPackage;
		ApiConnection = apiConnection;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(App app, AppRemote appRemote, AppLicenseKey? appLicenseKey, AppPassword? appPassword, AppValidationMode appValidationMode, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey, HandleExceptionAndReturnDelegate<AppLicenseKeyRequired> onAppLicenseKeyRequired, HandleExceptionAndReturnDelegate<InvalidAppLicenseKey> onInvalidAppLicenseKey, HandleExceptionAndReturnDelegate<BlockedAppLicenseKey> onBlockedAppLicenseKey, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(appLicenseKey, "appLicenseKey");
		Arg.IsValid(appPassword, "appPassword");
		LogInfo("Executing DoesDirHaveFreeSpaceForUpdatingApp...");
		LogTrace("app = " + app);
		LogTrace("appRemote = " + appRemote);
		AppLicenseKey? appLicenseKey2 = appLicenseKey;
		LogTrace("appLicenseKey = " + (appLicenseKey2.HasValue ? ((string)appLicenseKey2.GetValueOrDefault()) : null));
		AppPassword? appPassword2 = appPassword;
		LogTrace("appPassword = " + (appPassword2.HasValue ? ((string)appPassword2.GetValueOrDefault()) : null));
		LogTrace("appValidationMode = " + appValidationMode);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(app, appRemote, appLicenseKey, appPassword, appValidationMode, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: success.");
			return result;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			object obj = onAppRootDirDoesNotExist(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			object obj2 = onUnauthorizedAccessToAppRootDir(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: InvalidAppAuthKey.");
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
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: ExpiredAppAuthKey.");
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
		catch (AppLicenseKeyRequired e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: AppLicenseKeyRequired.");
			if (onAppLicenseKeyRequired == null)
			{
				throw;
			}
			object obj5 = onAppLicenseKeyRequired(e5);
			if (obj5 == null)
			{
				return false;
			}
			return (bool)obj5;
		}
		catch (InvalidAppLicenseKey e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: InvalidAppLicenseKey.");
			if (onInvalidAppLicenseKey == null)
			{
				throw;
			}
			object obj6 = onInvalidAppLicenseKey(e6);
			if (obj6 == null)
			{
				return false;
			}
			return (bool)obj6;
		}
		catch (BlockedAppLicenseKey e7)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: BlockedAppLicenseKey.");
			if (onBlockedAppLicenseKey == null)
			{
				throw;
			}
			object obj7 = onBlockedAppLicenseKey(e7);
			if (obj7 == null)
			{
				return false;
			}
			return (bool)obj7;
		}
		catch (InvalidAppPassword e8)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj8 = onInvalidAppPassword(e8);
			if (obj8 == null)
			{
				return false;
			}
			return (bool)obj8;
		}
		catch (AppRemoteNotFound e9)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj9 = onAppRemoteNotFound(e9);
			if (obj9 == null)
			{
				return false;
			}
			return (bool)obj9;
		}
		catch (AppRemoteVersionNotFound e10)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj10 = onAppRemoteVersionNotFound(e10);
			if (obj10 == null)
			{
				return false;
			}
			return (bool)obj10;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesDirHaveFreeSpaceForUpdatingApp has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DoesDirHaveFreeSpaceForUpdatingApp failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DoesDirHaveFreeSpaceForUpdatingApp finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DoesDirHaveFreeSpaceForUpdatingAppDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DoesDirHaveFreeSpaceForUpdatingApp(context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<GetAppRemoteLatestVersionIdDelegate>(), context.Resolve<IsAppRemoteLicenseSecretRequiredDelegate>(), context.Resolve<IsAppRemoteAuthTokenRequiredDelegate>(), context.Resolve<IsAppRemotePasswordRequiredDelegate>(), context.Resolve<GetAppLicenseKeyDelegate>(), context.Resolve<GetAppPasswordDelegate>(), context.Resolve<ValidateAppLicenseKeyBDelegate>(), context.Resolve<GetUpdateStrategyDelegate>(), context.Resolve<GetFileUpdateStrategiesPackagesInfoDelegate>(), context.Resolve<GetDirFreeSpaceBytesCountDelegate>(), context.Resolve<GetPackageDelegate>(), context.Resolve<IApiConnection>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
