using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteContentDataFileInfo
{
	public GetAppRemoteContentInfoDelegate GetAppRemoteContentInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageDataFileInfo? Execute(AppRemote appRemote, AppVersionId versionId, AppLicenseSecret? licenseSecret, AppPassword? password, AppPath path, CancellationToken cancellationToken)
	{
		return GetAppRemoteContentInfo(appRemote, versionId, licenseSecret, password, cancellationToken, null, null, null, null).FilesInfo.FirstOrNull((AppRemotePackageFileInfo x) => x.DataFileInfo.HasValue && (string)x.DataFileInfo.Value.Path == (string)path)?.DataFileInfo;
	}

	public GetAppRemoteContentDataFileInfo(GetAppRemoteContentInfoDelegate getAppRemoteContentInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteContentInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteContentInfo = getAppRemoteContentInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageDataFileInfo? DelegateExecute(AppRemote appRemote, AppVersionId versionId, AppLicenseSecret? licenseSecret, AppPassword? password, AppPath path, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetAppRemoteContentDataFileInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		LogTrace("path = " + path);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemotePackageDataFileInfo? appRemotePackageDataFileInfo = Execute(appRemote, versionId, licenseSecret, password, path, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemotePackageDataFileInfo? appRemotePackageDataFileInfo2 = appRemotePackageDataFileInfo;
			LogTrace("result = " + appRemotePackageDataFileInfo2.ToString());
			LogInfo("GetAppRemoteContentDataFileInfo finished: success.");
			return appRemotePackageDataFileInfo;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteContentDataFileInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return null;
			}
			return (AppRemotePackageDataFileInfo?)obj;
		}
		catch (InvalidAppPassword e2)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteContentDataFileInfo finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj2 = onInvalidAppPassword(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (AppRemotePackageDataFileInfo?)obj2;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteContentDataFileInfo finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj3 = onInvalidAppAuthKey(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (AppRemotePackageDataFileInfo?)obj3;
		}
		catch (ExpiredAppAuthKey e4)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteContentDataFileInfo finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj4 = onExpiredAppAuthKey(e4);
			if (obj4 == null)
			{
				return null;
			}
			return (AppRemotePackageDataFileInfo?)obj4;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteContentDataFileInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteContentDataFileInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteContentDataFileInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteContentDataFileInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteContentDataFileInfo(context.Resolve<GetAppRemoteContentInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
