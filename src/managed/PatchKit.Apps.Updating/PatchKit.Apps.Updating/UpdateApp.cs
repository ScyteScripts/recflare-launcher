using System;
using System.Threading;
using Autofac;
using PatchKit.Api;
using PatchKit.Apps.Remote;
using PatchKit.Apps.Updating.Strategy;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class UpdateApp
{
	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public GetAppRemoteLatestVersionIdDelegate GetAppRemoteLatestVersionId { get; }

	public IsAppRemoteLicenseSecretRequiredDelegate IsAppRemoteLicenseSecretRequired { get; }

	public IsAppRemoteAuthTokenRequiredDelegate IsAppRemoteAuthTokenRequired { get; }

	public IsAppRemotePasswordRequiredDelegate IsAppRemotePasswordRequired { get; }

	public GetAppLicenseKeyDelegate GetAppLicenseKey { get; }

	public GetAppPasswordDelegate GetAppPassword { get; }

	public SetAppPasswordDelegate SetAppPassword { get; }

	public ValidateAndSaveAppLicenseKeyDelegate ValidateAndSaveAppLicenseKey { get; }

	public GetUpdateStrategyDelegate GetUpdateStrategy { get; }

	public GetAppInstalledVersionIdDelegate GetAppInstalledVersionId { get; }

	public SetAppInstalledVersionIdDelegate SetAppInstalledVersionId { get; }

	public SetAppLastInstalledVersionIdDelegate SetAppLastInstalledVersionId { get; }

	public UpdateAppFilesDelegate UpdateAppFiles { get; }

	public CreateAppDirsDelegate CreateAppDirs { get; }

	public DeleteRedundantAppEntriesDelegate DeleteRedundantAppEntries { get; }

	public WriteAppMetaDataCacheDelegate WriteAppMetaDataCache { get; }

	public UpdateAppStartInfoDelegate UpdateAppStartInfo { get; }

	public AddExecutableRightsToAppFilesDelegate AddExecutableRightsToAppFiles { get; }

	public IApiConnection ApiConnection { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppRemote appRemote, AppLicenseKey? appLicenseKey, AppPassword? appPassword, AppValidationMode appValidationMode, ReportProgressDelegate<UpdateAppProgress> reportProgress, CancellationToken cancellationToken)
	{
		if (!DoesAppRootDirExist(app, null))
		{
			throw new AppRootDirDoesNotExist();
		}
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
			licenseSecret = ValidateAndSaveAppLicenseKey(app, appRemote.Secret, appLicenseKey.Value, cancellationToken, null, null, (UnauthorizedAccess e) =>
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
			SetAppPassword(app, appPassword.Value, appRemote.Secret, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppRootDir();
			});
		}
		UpdateAppContext context = new UpdateAppContext(app, appRemote, targetVersionId, licenseSecret, appPassword);
		bool flag4 = false;
		do
		{
			UpdateStrategy updateStrategy = GetUpdateStrategy(context, appValidationMode, cancellationToken, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppRootDir();
			}, null, null, null, null, null);
			AppVersionId? value = GetAppInstalledVersionId(context.App, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null);
			if (value.HasValue)
			{
				SetAppLastInstalledVersionId(context.App, value, (AppRootDirDoesNotExist e) =>
				{
					throw new UnpredictedException(e);
				}, null);
			}
			SetAppInstalledVersionId(context.App, null, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null);
			WriteAppMetaDataCache(context.App, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppRootDir();
			});
			try
			{
				DeleteRedundantAppEntries(context.App, updateStrategy.EntriesToDelete, (UnauthorizedAccess e) =>
				{
					throw new UnauthorizedAccessToAppRootDir();
				});
				UpdateAppFiles(context, updateStrategy.FileStrategies, reportProgress, cancellationToken, (UnauthorizedAccess e) =>
				{
					throw new UnauthorizedAccessToAppRootDir();
				}, null, (OutOfFreeDiskSpace e) =>
				{
					throw new OutOfAppRootDirFreeSpace();
				}, null, null, null, null);
				CreateAppDirs(context.App, context.TargetVersionId, updateStrategy.DirsToCreate, (UnauthorizedAccess e) =>
				{
					throw new UnauthorizedAccessToAppRootDir();
				});
				flag4 = true;
			}
			catch (UpdateAppFailure exception)
			{
				switch (appValidationMode)
				{
				case AppValidationMode.MetaData:
					appValidationMode = AppValidationMode.FileSystem;
					break;
				case AppValidationMode.FileSystem:
					appValidationMode = AppValidationMode.Hashes;
					break;
				}
				LogWarning($"Update has failed. Retrying with higher validation mode: {appValidationMode}", exception);
			}
			finally
			{
				WriteAppMetaDataCache(context.App, (UnauthorizedAccess e) =>
				{
					throw new UnauthorizedAccessToAppRootDir();
				});
			}
		}
		while (!flag4);
		SetAppInstalledVersionId(context.App, context.TargetVersionId, (AppRootDirDoesNotExist e) =>
		{
			throw new UnpredictedException(e);
		}, null);
		SetAppLastInstalledVersionId(context.App, context.TargetVersionId, (AppRootDirDoesNotExist e) =>
		{
			throw new UnpredictedException(e);
		}, null);
		WriteAppMetaDataCache(context.App, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
		UpdateAppStartInfo(context, cancellationToken, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}, null);
		AddExecutableRightsToAppFiles(context.App, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
		WriteAppMetaDataCache(context.App, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
	}

	public UpdateApp(DoesAppRootDirExistDelegate doesAppRootDirExist, GetAppRemoteLatestVersionIdDelegate getAppRemoteLatestVersionId, IsAppRemoteLicenseSecretRequiredDelegate isAppRemoteLicenseSecretRequired, IsAppRemoteAuthTokenRequiredDelegate isAppRemoteAuthTokenRequired, IsAppRemotePasswordRequiredDelegate isAppRemotePasswordRequired, GetAppLicenseKeyDelegate getAppLicenseKey, GetAppPasswordDelegate getAppPassword, SetAppPasswordDelegate setAppPassword, ValidateAndSaveAppLicenseKeyDelegate validateAndSaveAppLicenseKey, GetUpdateStrategyDelegate getUpdateStrategy, GetAppInstalledVersionIdDelegate getAppInstalledVersionId, SetAppInstalledVersionIdDelegate setAppInstalledVersionId, SetAppLastInstalledVersionIdDelegate setAppLastInstalledVersionId, UpdateAppFilesDelegate updateAppFiles, CreateAppDirsDelegate createAppDirs, DeleteRedundantAppEntriesDelegate deleteRedundantAppEntries, WriteAppMetaDataCacheDelegate writeAppMetaDataCache, UpdateAppStartInfoDelegate updateAppStartInfo, AddExecutableRightsToAppFilesDelegate addExecutableRightsToAppFiles, IApiConnection apiConnection, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(getAppRemoteLatestVersionId);
		Assert.IsNotNull(isAppRemoteLicenseSecretRequired);
		Assert.IsNotNull(isAppRemoteAuthTokenRequired);
		Assert.IsNotNull(isAppRemotePasswordRequired);
		Assert.IsNotNull(getAppLicenseKey);
		Assert.IsNotNull(getAppPassword);
		Assert.IsNotNull(setAppPassword);
		Assert.IsNotNull(validateAndSaveAppLicenseKey);
		Assert.IsNotNull(getUpdateStrategy);
		Assert.IsNotNull(getAppInstalledVersionId);
		Assert.IsNotNull(setAppInstalledVersionId);
		Assert.IsNotNull(setAppLastInstalledVersionId);
		Assert.IsNotNull(updateAppFiles);
		Assert.IsNotNull(createAppDirs);
		Assert.IsNotNull(deleteRedundantAppEntries);
		Assert.IsNotNull(writeAppMetaDataCache);
		Assert.IsNotNull(updateAppStartInfo);
		Assert.IsNotNull(addExecutableRightsToAppFiles);
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
		SetAppPassword = setAppPassword;
		ValidateAndSaveAppLicenseKey = validateAndSaveAppLicenseKey;
		GetUpdateStrategy = getUpdateStrategy;
		GetAppInstalledVersionId = getAppInstalledVersionId;
		SetAppInstalledVersionId = setAppInstalledVersionId;
		SetAppLastInstalledVersionId = setAppLastInstalledVersionId;
		UpdateAppFiles = updateAppFiles;
		CreateAppDirs = createAppDirs;
		DeleteRedundantAppEntries = deleteRedundantAppEntries;
		WriteAppMetaDataCache = writeAppMetaDataCache;
		UpdateAppStartInfo = updateAppStartInfo;
		AddExecutableRightsToAppFiles = addExecutableRightsToAppFiles;
		ApiConnection = apiConnection;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppRemote appRemote, AppLicenseKey? appLicenseKey, AppPassword? appPassword, AppValidationMode appValidationMode, ReportProgressDelegate<UpdateAppProgress> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey, HandleExceptionDelegate<AppLicenseKeyRequired> onAppLicenseKeyRequired, HandleExceptionDelegate<InvalidAppLicenseKey> onInvalidAppLicenseKey, HandleExceptionDelegate<BlockedAppLicenseKey> onBlockedAppLicenseKey, HandleExceptionDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionDelegate<AppRemoteNotFound> onAppRemoteNotFound, HandleExceptionDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionDelegate<OutOfAppRootDirFreeSpace> onOutOfAppRootDirFreeSpace)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(appLicenseKey, "appLicenseKey");
		Arg.IsValid(appPassword, "appPassword");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing UpdateApp...");
		LogTrace("app = " + app);
		LogTrace("appRemote = " + appRemote);
		AppLicenseKey? appLicenseKey2 = appLicenseKey;
		LogTrace("appLicenseKey = " + (appLicenseKey2.HasValue ? ((string)appLicenseKey2.GetValueOrDefault()) : null));
		AppPassword? appPassword2 = appPassword;
		LogTrace("appPassword = " + (appPassword2.HasValue ? ((string)appPassword2.GetValueOrDefault()) : null));
		LogTrace("appValidationMode = " + appValidationMode);
		LogTrace("reportProgress = " + reportProgress);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, appRemote, appLicenseKey, appPassword, appValidationMode, reportProgress, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: success.");
			return null;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			onAppRootDirDoesNotExist(e);
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			onUnauthorizedAccessToAppRootDir(e2);
			return null;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			onInvalidAppAuthKey(e3);
			return null;
		}
		catch (ExpiredAppAuthKey e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			onExpiredAppAuthKey(e4);
			return null;
		}
		catch (AppLicenseKeyRequired e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: AppLicenseKeyRequired.");
			if (onAppLicenseKeyRequired == null)
			{
				throw;
			}
			onAppLicenseKeyRequired(e5);
			return null;
		}
		catch (InvalidAppLicenseKey e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: InvalidAppLicenseKey.");
			if (onInvalidAppLicenseKey == null)
			{
				throw;
			}
			onInvalidAppLicenseKey(e6);
			return null;
		}
		catch (BlockedAppLicenseKey e7)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: BlockedAppLicenseKey.");
			if (onBlockedAppLicenseKey == null)
			{
				throw;
			}
			onBlockedAppLicenseKey(e7);
			return null;
		}
		catch (InvalidAppPassword e8)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			onInvalidAppPassword(e8);
			return null;
		}
		catch (AppRemoteNotFound e9)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			onAppRemoteNotFound(e9);
			return null;
		}
		catch (AppRemoteVersionNotFound e10)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			onAppRemoteVersionNotFound(e10);
			return null;
		}
		catch (OutOfAppRootDirFreeSpace e11)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp finished: OutOfAppRootDirFreeSpace.");
			if (onOutOfAppRootDirFreeSpace == null)
			{
				throw;
			}
			onOutOfAppRootDirFreeSpace(e11);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateApp has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("UpdateApp failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("UpdateApp finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public UpdateAppDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new UpdateApp(context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<GetAppRemoteLatestVersionIdDelegate>(), context.Resolve<IsAppRemoteLicenseSecretRequiredDelegate>(), context.Resolve<IsAppRemoteAuthTokenRequiredDelegate>(), context.Resolve<IsAppRemotePasswordRequiredDelegate>(), context.Resolve<GetAppLicenseKeyDelegate>(), context.Resolve<GetAppPasswordDelegate>(), context.Resolve<SetAppPasswordDelegate>(), context.Resolve<ValidateAndSaveAppLicenseKeyDelegate>(), context.Resolve<GetUpdateStrategyDelegate>(), context.Resolve<GetAppInstalledVersionIdDelegate>(), context.Resolve<SetAppInstalledVersionIdDelegate>(), context.Resolve<SetAppLastInstalledVersionIdDelegate>(), context.Resolve<UpdateAppFilesDelegate>(), context.Resolve<CreateAppDirsDelegate>(), context.Resolve<DeleteRedundantAppEntriesDelegate>(), context.Resolve<WriteAppMetaDataCacheDelegate>(), context.Resolve<UpdateAppStartInfoDelegate>(), context.Resolve<AddExecutableRightsToAppFilesDelegate>(), context.Resolve<IApiConnection>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
