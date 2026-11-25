using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Apps.Updating.Strategy;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public class GetFileUpdateStrategiesPackagesInfo
{
	public GetFilteredNewAppRemoteContentInfoDelegate GetFilteredNewAppRemoteContentInfo { get; }

	public GetFilteredNewAppRemoteDiffInfoDelegate GetFilteredNewAppRemoteDiffInfo { get; }

	public DoesAppRemoteDiffContainsPatchFileDelegate DoesAppRemoteDiffContainsPatchFile { get; }

	public DoesAppRemoteDiffContainsDataFileDelegate DoesAppRemoteDiffContainsDataFile { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppRemotePackageInfo> Execute(AppRemote appRemote, AppVersionId targetVersionId, AppLicenseSecret? licenseSecret, AppPassword? password, ImmutableArray<FileUpdateStrategy> strategies, CancellationToken cancellationToken)
	{
		List<AppPath> list = new List<AppPath>();
		List<AppPath> list2 = new List<AppPath>();
		Dictionary<AppVersionPatchLinkId, List<AppPath>> dictionary = new Dictionary<AppVersionPatchLinkId, List<AppPath>>();
		bool flag = (int)targetVersionId > 1 && strategies.FirstOrNull((FileUpdateStrategy x) => x.PatchesStrategy.HasValue && (int)x.PatchesStrategy.Value.SourceVersionId == (int)new AppVersionId((int)targetVersionId - 1)).HasValue;
		AppVersionPatchLinkId? appVersionPatchLinkId = (((int)targetVersionId > 1) ? new AppVersionPatchLinkId?(new AppVersionPatchLinkId(new AppVersionId((int)targetVersionId - 1), targetVersionId)) : ((AppVersionPatchLinkId?)null));
		foreach (FileUpdateStrategy item2 in strategies)
		{
			if (item2.DataStrategy.HasValue)
			{
				FileUpdateDataStrategy value = item2.DataStrategy.Value;
				if ((appVersionPatchLinkId.HasValue & flag) && DoesAppRemoteDiffContainsDataFile(appRemote, appVersionPatchLinkId.Value, licenseSecret, password, value.Path, cancellationToken, (AppRemoteVersionPatchLinkNotFound _) => false, null, null, null))
				{
					list2.Add(value.Path);
				}
				else
				{
					list.Add(value.Path);
				}
			}
			else
			{
				if (!item2.PatchesStrategy.HasValue)
				{
					continue;
				}
				FileUpdatePatchesStrategy value2 = item2.PatchesStrategy.Value;
				AppVersionPatchLinkId appVersionPatchLinkId2 = new AppVersionPatchLinkId(value2.SourceVersionId, targetVersionId);
				DoesAppRemoteDiffContainsPatchFileDelegate doesAppRemoteDiffContainsPatchFile = DoesAppRemoteDiffContainsPatchFile;
				AppPath path = value2.Path;
				if (doesAppRemoteDiffContainsPatchFile(appRemote, appVersionPatchLinkId2, licenseSecret, password, path, cancellationToken, (AppRemoteVersionPatchLinkNotFound _) => false, null, null, null))
				{
					if (!dictionary.ContainsKey(appVersionPatchLinkId2))
					{
						dictionary.Add(appVersionPatchLinkId2, new List<AppPath>());
					}
					Assert.IsNotNull(dictionary[appVersionPatchLinkId2]);
					dictionary[appVersionPatchLinkId2].Add(value2.Path);
					continue;
				}
				for (int num = value2.SourceVersionId; num < (int)targetVersionId; num++)
				{
					AppVersionId sourceVersionId = new AppVersionId(num);
					AppVersionId targetVersionId2 = new AppVersionId(num + 1);
					AppVersionPatchLinkId key = new AppVersionPatchLinkId(sourceVersionId, targetVersionId2);
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, new List<AppPath>());
					}
					Assert.IsNotNull(dictionary[key]);
					dictionary[key].Add(value2.Path);
				}
			}
		}
		AppRemotePackageInfo? appRemotePackageInfo = null;
		if (list.Count > 0)
		{
			appRemotePackageInfo = GetFilteredNewAppRemoteContentInfo(appRemote, targetVersionId, licenseSecret, password, list.AsImmutable(), cancellationToken, null, null, null, null);
		}
		List<AppRemotePackageInfo> list3 = new List<AppRemotePackageInfo>();
		foreach (KeyValuePair<AppVersionPatchLinkId, List<AppPath>> item3 in dictionary)
		{
			Assert.IsNotNull(item3.Value);
			bool v = item3.Value.Count > 0;
			Assert.IsTrue(v, "hasAnyDiffPatchFiles");
			ImmutableArray<AppPath> dataFiles = (item3.Key.Equals(appVersionPatchLinkId) ? list2.AsImmutable() : ImmutableArray<AppPath>.Empty);
			AppRemotePackageInfo item = GetFilteredNewAppRemoteDiffInfo(appRemote, item3.Key, licenseSecret, password, dataFiles, item3.Value.AsImmutable(), cancellationToken, (AppRemoteVersionPatchLinkNotFound e) =>
			{
				throw new UnpredictedException(e);
			}, null, null, null);
			list3.Add(item);
		}
		List<AppRemotePackageInfo> list4 = new List<AppRemotePackageInfo>();
		if (appRemotePackageInfo.HasValue)
		{
			list4.Add(appRemotePackageInfo.Value);
		}
		list4.AddRange(list3);
		return list4.AsImmutable();
	}

	public GetFileUpdateStrategiesPackagesInfo(GetFilteredNewAppRemoteContentInfoDelegate getFilteredNewAppRemoteContentInfo, GetFilteredNewAppRemoteDiffInfoDelegate getFilteredNewAppRemoteDiffInfo, DoesAppRemoteDiffContainsPatchFileDelegate doesAppRemoteDiffContainsPatchFile, DoesAppRemoteDiffContainsDataFileDelegate doesAppRemoteDiffContainsDataFile, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredNewAppRemoteContentInfo);
		Assert.IsNotNull(getFilteredNewAppRemoteDiffInfo);
		Assert.IsNotNull(doesAppRemoteDiffContainsPatchFile);
		Assert.IsNotNull(doesAppRemoteDiffContainsDataFile);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredNewAppRemoteContentInfo = getFilteredNewAppRemoteContentInfo;
		GetFilteredNewAppRemoteDiffInfo = getFilteredNewAppRemoteDiffInfo;
		DoesAppRemoteDiffContainsPatchFile = doesAppRemoteDiffContainsPatchFile;
		DoesAppRemoteDiffContainsDataFile = doesAppRemoteDiffContainsDataFile;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppRemotePackageInfo> DelegateExecute(AppRemote appRemote, AppVersionId targetVersionId, AppLicenseSecret? licenseSecret, AppPassword? password, ImmutableArray<FileUpdateStrategy> strategies, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(targetVersionId, "targetVersionId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		Arg.IsValid(strategies, "strategies");
		LogInfo("Executing GetFileUpdateStrategiesPackagesInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = targetVersionId;
		LogTrace("targetVersionId = " + appVersionId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		ImmutableArray<FileUpdateStrategy> immutableArray = strategies;
		LogTrace("strategies = " + immutableArray.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<AppRemotePackageInfo> immutableArray2 = Execute(appRemote, targetVersionId, licenseSecret, password, strategies, cancellationToken);
			EndLogRegion(enabled: true);
			ImmutableArray<AppRemotePackageInfo> immutableArray3 = immutableArray2;
			LogTrace("result = " + immutableArray3.ToString());
			LogInfo("GetFileUpdateStrategiesPackagesInfo finished: success.");
			return immutableArray2;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileUpdateStrategiesPackagesInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (ImmutableArray<AppRemotePackageInfo>)obj;
		}
		catch (InvalidAppPassword e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileUpdateStrategiesPackagesInfo finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj2 = onInvalidAppPassword(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (ImmutableArray<AppRemotePackageInfo>)obj2;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileUpdateStrategiesPackagesInfo finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj3 = onInvalidAppAuthKey(e3);
			if (obj3 == null)
			{
				return default;
			}
			return (ImmutableArray<AppRemotePackageInfo>)obj3;
		}
		catch (ExpiredAppAuthKey e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileUpdateStrategiesPackagesInfo finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj4 = onExpiredAppAuthKey(e4);
			if (obj4 == null)
			{
				return default;
			}
			return (ImmutableArray<AppRemotePackageInfo>)obj4;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFileUpdateStrategiesPackagesInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFileUpdateStrategiesPackagesInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFileUpdateStrategiesPackagesInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFileUpdateStrategiesPackagesInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFileUpdateStrategiesPackagesInfo(context.Resolve<GetFilteredNewAppRemoteContentInfoDelegate>(), context.Resolve<GetFilteredNewAppRemoteDiffInfoDelegate>(), context.Resolve<DoesAppRemoteDiffContainsPatchFileDelegate>(), context.Resolve<DoesAppRemoteDiffContainsDataFileDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
