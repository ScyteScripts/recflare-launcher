using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemoteContentInfo
{
	public GetAppRemoteContentInfoDelegate GetAppRemoteContentInfo { get; }

	public GetFilteredNewAppRemotePackageInfoBDelegate GetFilteredNewAppRemotePackageInfoB { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageInfo Execute(AppRemote appRemote, AppVersionId versionId, AppLicenseSecret? licenseSecret, AppPassword? password, ImmutableArray<AppPath> dataFiles, CancellationToken cancellationToken)
	{
		AppRemotePackageInfo info = GetAppRemoteContentInfo(appRemote, versionId, licenseSecret, password, cancellationToken, null, null, null, null);
		return GetFilteredNewAppRemotePackageInfoB(info, dataFiles, ImmutableArray<AppPath>.Empty);
	}

	public GetFilteredNewAppRemoteContentInfo(GetAppRemoteContentInfoDelegate getAppRemoteContentInfo, GetFilteredNewAppRemotePackageInfoBDelegate getFilteredNewAppRemotePackageInfoB, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteContentInfo);
		Assert.IsNotNull(getFilteredNewAppRemotePackageInfoB);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteContentInfo = getAppRemoteContentInfo;
		GetFilteredNewAppRemotePackageInfoB = getFilteredNewAppRemotePackageInfoB;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageInfo DelegateExecute(AppRemote appRemote, AppVersionId versionId, AppLicenseSecret? licenseSecret, AppPassword? password, ImmutableArray<AppPath> dataFiles, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		Arg.IsValid(dataFiles, "dataFiles");
		LogInfo("Executing GetFilteredNewAppRemoteContentInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		ImmutableArray<AppPath> immutableArray = dataFiles;
		LogTrace("dataFiles = " + immutableArray.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo = Execute(appRemote, versionId, licenseSecret, password, dataFiles, cancellationToken);
			EndLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo2 = appRemotePackageInfo;
			LogTrace("result = " + appRemotePackageInfo2.ToString());
			LogInfo("GetFilteredNewAppRemoteContentInfo finished: success.");
			return appRemotePackageInfo;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteContentInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (AppRemotePackageInfo)obj;
		}
		catch (InvalidAppPassword e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteContentInfo finished: InvalidAppPassword.");
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
			LogInfo("GetFilteredNewAppRemoteContentInfo finished: InvalidAppAuthKey.");
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
			LogInfo("GetFilteredNewAppRemoteContentInfo finished: ExpiredAppAuthKey.");
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
			LogInfo("GetFilteredNewAppRemoteContentInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemoteContentInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemoteContentInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemoteContentInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemoteContentInfo(context.Resolve<GetAppRemoteContentInfoDelegate>(), context.Resolve<GetFilteredNewAppRemotePackageInfoBDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
