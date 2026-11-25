using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using PatchKit.Api;
using PatchKit.Api.Models;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteDiffInfo
{
	public IApiConnection ApiConnection { get; }

	public GetAppRemotePackageUnpackPasswordDelegate GetAppRemotePackageUnpackPassword { get; }

	public GetAppRemotePack1UnpackInfoBDelegate GetAppRemotePack1UnpackInfoB { get; }

	public GetAppRemotePackageSourcesInfoDelegate GetAppRemotePackageSourcesInfo { get; }

	public GetCountryCodeDelegate GetCountryCode { get; }

	public ParseAppRemoteHashDelegate ParseAppRemoteHash { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageInfo Execute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, CancellationToken cancellationToken)
	{
		Dictionary<AppVersionPatchLinkId, AppRemotePackageInfo> diffInfoCache = appRemote.DiffInfoCache;
		if (diffInfoCache.ContainsKey(versionPatchLinkId))
		{
			return diffInfoCache[versionPatchLinkId];
		}
		Text? authHeaderValue = (appRemote.AuthKey.HasValue ? new Text?(new Text($"Bearer {appRemote.AuthKey.Value}")) : ((Text?)null));
		int? patchLinkId = null;
		if ((int)versionPatchLinkId.TargetVersionId != (int)versionPatchLinkId.SourceVersionId + 1)
		{
			AppVersionPatchLink? appVersionPatchLink = ApiConnection.GetAppVersion(appRemote.Secret, versionPatchLinkId.SourceVersionId, null, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken).PatchLinks.Outgoing.FirstOrNull((AppVersionPatchLink x) => x.TargetVersion == (int)versionPatchLinkId.TargetVersionId);
			if (!appVersionPatchLink.HasValue)
			{
				throw new AppRemoteVersionPatchLinkNotFound(appRemote.Secret, versionPatchLinkId);
			}
			patchLinkId = appVersionPatchLink.Value.Id;
		}
		AppDiffSummary? rawValue = GetRawValue(appRemote.Secret, versionPatchLinkId.TargetVersionId, patchLinkId, cancellationToken);
		if (!rawValue.HasValue)
		{
			throw new AppRemoteVersionPatchLinkNotFound(appRemote.Secret, versionPatchLinkId);
		}
		AppDiffSummary value = rawValue.Value;
		ImmutableArray<ResourceUrl>? rawUrlsValue = GetRawUrlsValue(appRemote.Secret, versionPatchLinkId.TargetVersionId, patchLinkId, licenseSecret, password, cancellationToken);
		if (!rawUrlsValue.HasValue)
		{
			throw new AppRemoteVersionPatchLinkNotFound(appRemote.Secret, versionPatchLinkId);
		}
		ImmutableArray<ResourceUrl> value2 = rawUrlsValue.Value;
		IEnumerable<AppRemotePackageFileInfo> first = value.AddedFiles.Where((string x) => !string.IsNullOrEmpty(x) && !x.EndsWith("/")).Select((Func<string, AppRemotePackageFileInfo>)((string x) => GetDataFileInfo(x, versionPatchLinkId)));
		ImmutableArray<string> unchangedFiles = value.UnchangedFiles;
		IEnumerable<AppRemotePackageFileInfo> second = value.ModifiedFiles.Where((string x) => !string.IsNullOrEmpty(x) && !x.EndsWith("/")).Select((Func<string, AppRemotePackageFileInfo>)((string x) => GetPatchFileInfo(x, unchangedFiles.Contains(x), versionPatchLinkId)));
		BytesCount size = new BytesCount(value.Size);
		AppRemotePackageUnpackInfo? v = null;
		Text text = GetAppRemotePackageUnpackPassword(appRemote, versionPatchLinkId.TargetVersionId);
		if (value.CompressionMethod == "zip")
		{
			v = new AppRemoteZipUnpackInfo(new AppRemoteZipUnpackPassword(text));
		}
		else if (value.CompressionMethod == "pack1")
		{
			v = GetAppRemotePack1UnpackInfoB(value2.Select(GetMetaUrl).AsImmutable(), new AppRemotePack1UnpackPassword(text), authHeaderValue, cancellationToken, null, null);
		}
		Assert.IsNotNull(v);
		AppRemotePackageChunksInfo? appRemotePackageChunksInfo = null;
		if (value.Chunks.Hashes.Length > 0)
		{
			appRemotePackageChunksInfo = new AppRemotePackageChunksInfo(new BytesCount(value.Chunks.Size), value.Chunks.Hashes.Select((string x) =>
			{
				Assert.IsNotNull(x);
				return ParseAppRemoteHash(new Text(x));
			}).AsImmutable());
		}
		AppRemotePackageInfo appRemotePackageInfo = new AppRemotePackageInfo(first.Concat(second).AsImmutable(), sourcesInfo: GetAppRemotePackageSourcesInfo(value2, size, authHeaderValue), chunksInfo: appRemotePackageChunksInfo, unpackInfo: v.Value, size: size);
		diffInfoCache[versionPatchLinkId] = appRemotePackageInfo;
		return appRemotePackageInfo;
	}

	private HttpAddress GetMetaUrl(ResourceUrl raw)
	{
		Assert.IsNotNull(raw.MetaUrl);
		return new HttpAddress(new Uri(raw.MetaUrl));
	}

	private AppRemotePackageDataFileInfo GetDataFileInfo(string raw, AppVersionPatchLinkId versionPatchLinkId)
	{
		Assert.IsNotNull(raw);
		return new AppRemotePackageDataFileInfo(new AppPath(raw), versionPatchLinkId.TargetVersionId);
	}

	private AppRemotePackagePatchFileInfo GetPatchFileInfo(string raw, bool isPatchEmpty, AppVersionPatchLinkId versionPatchLinkId)
	{
		Assert.IsNotNull(raw);
		AppPath path = new AppPath(raw);
		bool isPatchEmpty2 = isPatchEmpty;
		return new AppRemotePackagePatchFileInfo(path, versionPatchLinkId.SourceVersionId, versionPatchLinkId.TargetVersionId, isPatchEmpty2);
	}

	private AppDiffSummary? GetRawValue(AppSecret secret, AppVersionId targetVerisonId, int? patchLinkId, CancellationToken cancellationToken)
	{
		AppDiffSummary? appDiffSummary = null;
		while (!appDiffSummary.HasValue)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				appDiffSummary = ((!patchLinkId.HasValue) ? new AppDiffSummary?(ApiConnection.GetAppVersionDiffSummary(secret, targetVerisonId, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken)) : new AppDiffSummary?(ApiConnection.GetAppVersionPatchLinkDiffSummary(secret, targetVerisonId, patchLinkId.Value, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken)));
			}
			catch (ApiConnectionException)
			{
			}
			catch (ApiResponseException ex2)
			{
				if (ex2.StatusCode == 404)
				{
					return null;
				}
			}
		}
		return appDiffSummary.Value;
	}

	private ImmutableArray<ResourceUrl>? GetRawUrlsValue(AppSecret secret, AppVersionId targetVerisonId, int? patchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, CancellationToken cancellationToken)
	{
		ImmutableArray<ResourceUrl>? immutableArray = null;
		Text? text = GetCountryCode();
		while (!immutableArray.HasValue)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				if (patchLinkId.HasValue)
				{
					IApiConnection apiConnection = ApiConnection;
					string appSecret = secret;
					int versionId = targetVerisonId;
					int value = patchLinkId.Value;
					Text? text2 = text;
					string country = (text2.HasValue ? ((string)text2.GetValueOrDefault()) : null);
					AppLicenseSecret? appLicenseSecret = licenseSecret;
					string keySecret = (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null);
					AppPassword? appPassword = password;
					immutableArray = apiConnection.GetAppVersionPatchLinkDiffUrls(appSecret, versionId, value, country, keySecret, appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken);
				}
				else
				{
					IApiConnection apiConnection2 = ApiConnection;
					string appSecret2 = secret;
					int versionId2 = targetVerisonId;
					Text? text2 = text;
					string country2 = (text2.HasValue ? ((string)text2.GetValueOrDefault()) : null);
					AppLicenseSecret? appLicenseSecret = licenseSecret;
					string keySecret2 = (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null);
					AppPassword? appPassword = password;
					immutableArray = apiConnection2.GetAppVersionDiffUrls(appSecret2, versionId2, country2, keySecret2, appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken);
				}
			}
			catch (ApiConnectionException)
			{
			}
			catch (ApiResponseException ex2)
			{
				if (ex2.StatusCode == 403)
				{
					throw new InvalidAppPassword();
				}
				if (ex2.StatusCode == 404)
				{
					return null;
				}
			}
		}
		return immutableArray.Value;
	}

	public GetAppRemoteDiffInfo(IApiConnection apiConnection, GetAppRemotePackageUnpackPasswordDelegate getAppRemotePackageUnpackPassword, GetAppRemotePack1UnpackInfoBDelegate getAppRemotePack1UnpackInfoB, GetAppRemotePackageSourcesInfoDelegate getAppRemotePackageSourcesInfo, GetCountryCodeDelegate getCountryCode, ParseAppRemoteHashDelegate parseAppRemoteHash, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(apiConnection);
		Assert.IsNotNull(getAppRemotePackageUnpackPassword);
		Assert.IsNotNull(getAppRemotePack1UnpackInfoB);
		Assert.IsNotNull(getAppRemotePackageSourcesInfo);
		Assert.IsNotNull(getCountryCode);
		Assert.IsNotNull(parseAppRemoteHash);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ApiConnection = apiConnection;
		GetAppRemotePackageUnpackPassword = getAppRemotePackageUnpackPassword;
		GetAppRemotePack1UnpackInfoB = getAppRemotePack1UnpackInfoB;
		GetAppRemotePackageSourcesInfo = getAppRemotePackageSourcesInfo;
		GetCountryCode = getCountryCode;
		ParseAppRemoteHash = parseAppRemoteHash;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageInfo DelegateExecute(AppRemote appRemote, AppVersionPatchLinkId versionPatchLinkId, AppLicenseSecret? licenseSecret, AppPassword? password, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionPatchLinkNotFound> onAppRemoteVersionPatchLinkNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionPatchLinkId, "versionPatchLinkId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		LogInfo("Executing GetAppRemoteDiffInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionPatchLinkId appVersionPatchLinkId = versionPatchLinkId;
		LogTrace("versionPatchLinkId = " + appVersionPatchLinkId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemotePackageInfo appRemotePackageInfo = Execute(appRemote, versionPatchLinkId, licenseSecret, password, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemotePackageInfo appRemotePackageInfo2 = appRemotePackageInfo;
			LogTrace("result = " + appRemotePackageInfo2.ToString());
			LogInfo("GetAppRemoteDiffInfo finished: success.");
			return appRemotePackageInfo;
		}
		catch (AppRemoteVersionPatchLinkNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffInfo finished: AppRemoteVersionPatchLinkNotFound.");
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
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffInfo finished: InvalidAppPassword.");
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
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffInfo finished: InvalidAppAuthKey.");
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
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffInfo finished: ExpiredAppAuthKey.");
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
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteDiffInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteDiffInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteDiffInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteDiffInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteDiffInfo(context.Resolve<IApiConnection>(), context.Resolve<GetAppRemotePackageUnpackPasswordDelegate>(), context.Resolve<GetAppRemotePack1UnpackInfoBDelegate>(), context.Resolve<GetAppRemotePackageSourcesInfoDelegate>(), context.Resolve<GetCountryCodeDelegate>(), context.Resolve<ParseAppRemoteHashDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
