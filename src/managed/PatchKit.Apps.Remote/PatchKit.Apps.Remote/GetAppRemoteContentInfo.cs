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

public class GetAppRemoteContentInfo
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

	private AppRemotePackageInfo Execute(AppRemote appRemote, AppVersionId versionId, AppLicenseSecret? licenseSecret, AppPassword? password, CancellationToken cancellationToken)
	{
		Dictionary<AppVersionId, AppRemotePackageInfo> contentInfoCache = appRemote.ContentInfoCache;
		if (contentInfoCache.ContainsKey(versionId))
		{
			return contentInfoCache[versionId];
		}
		Text? authHeaderValue = (appRemote.AuthKey.HasValue ? new Text?(new Text($"Bearer {appRemote.AuthKey.Value}")) : ((Text?)null));
		AppContentSummary rawValue = GetRawValue(appRemote.Secret, versionId, cancellationToken);
		ImmutableArray<ResourceUrl> rawUrlsValue = GetRawUrlsValue(appRemote.Secret, versionId, licenseSecret, password, cancellationToken);
		IEnumerable<AppRemotePackageFileInfo> enumerable = ((IEnumerable<AppContentSummaryFile>)rawValue.Files).Select((Func<AppContentSummaryFile, AppRemotePackageFileInfo>)((AppContentSummaryFile x) => GetDataFileInfo(x.Path, versionId)));
		BytesCount size = new BytesCount(rawValue.Size);
		AppRemotePackageUnpackInfo? v = null;
		Text text = GetAppRemotePackageUnpackPassword(appRemote, versionId);
		if (rawValue.CompressionMethod == "zip")
		{
			v = new AppRemoteZipUnpackInfo(new AppRemoteZipUnpackPassword(text));
		}
		else if (rawValue.CompressionMethod == "pack1")
		{
			v = GetAppRemotePack1UnpackInfoB(rawUrlsValue.Select(GetMetaUrl).AsImmutable(), new AppRemotePack1UnpackPassword(text), authHeaderValue, cancellationToken, null, null);
		}
		Assert.IsNotNull(v);
		AppRemotePackageChunksInfo? appRemotePackageChunksInfo = null;
		if (rawValue.Chunks.Hashes.Length > 0)
		{
			appRemotePackageChunksInfo = new AppRemotePackageChunksInfo(new BytesCount(rawValue.Chunks.Size), rawValue.Chunks.Hashes.Select((string x) =>
			{
				Assert.IsNotNull(x);
				return ParseAppRemoteHash(new Text(x));
			}).AsImmutable());
		}
		AppRemotePackageInfo appRemotePackageInfo = new AppRemotePackageInfo(enumerable.AsImmutable(), sourcesInfo: GetAppRemotePackageSourcesInfo(rawUrlsValue, size, authHeaderValue), chunksInfo: appRemotePackageChunksInfo, unpackInfo: v.Value, size: size);
		contentInfoCache[versionId] = appRemotePackageInfo;
		return appRemotePackageInfo;
	}

	private HttpAddress GetMetaUrl(ResourceUrl raw)
	{
		Assert.IsNotNull(raw.MetaUrl);
		return new HttpAddress(new Uri(raw.MetaUrl));
	}

	private AppRemotePackageDataFileInfo GetDataFileInfo(string raw, AppVersionId versionId)
	{
		Assert.IsNotNull(raw);
		return new AppRemotePackageDataFileInfo(new AppPath(raw), versionId);
	}

	private AppContentSummary GetRawValue(AppSecret secret, AppVersionId versionId, CancellationToken cancellationToken)
	{
		AppContentSummary? appContentSummary = null;
		while (!appContentSummary.HasValue)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				appContentSummary = ApiConnection.GetAppVersionContentSummary(secret, versionId, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken);
			}
			catch (ApiConnectionException)
			{
			}
			catch (ApiResponseException ex2)
			{
				if (ex2.StatusCode == 404)
				{
					throw new AppRemoteVersionNotFound(secret, versionId);
				}
			}
		}
		return appContentSummary.Value;
	}

	private ImmutableArray<ResourceUrl> GetRawUrlsValue(AppSecret secret, AppVersionId versionId, AppLicenseSecret? licenseSecret, AppPassword? password, CancellationToken cancellationToken)
	{
		ImmutableArray<ResourceUrl>? immutableArray = null;
		Text? text = GetCountryCode();
		while (!immutableArray.HasValue)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				IApiConnection apiConnection = ApiConnection;
				string appSecret = secret;
				int versionId2 = versionId;
				Text? text2 = text;
				string country = (text2.HasValue ? ((string)text2.GetValueOrDefault()) : null);
				AppLicenseSecret? appLicenseSecret = licenseSecret;
				string keySecret = (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null);
				AppPassword? appPassword = password;
				immutableArray = apiConnection.GetAppVersionContentUrls(appSecret, versionId2, country, keySecret, appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken);
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
					throw new AppRemoteVersionNotFound(secret, versionId);
				}
			}
		}
		return immutableArray.Value;
	}

	public GetAppRemoteContentInfo(IApiConnection apiConnection, GetAppRemotePackageUnpackPasswordDelegate getAppRemotePackageUnpackPassword, GetAppRemotePack1UnpackInfoBDelegate getAppRemotePack1UnpackInfoB, GetAppRemotePackageSourcesInfoDelegate getAppRemotePackageSourcesInfo, GetCountryCodeDelegate getCountryCode, ParseAppRemoteHashDelegate parseAppRemoteHash, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

	private AppRemotePackageInfo DelegateExecute(AppRemote appRemote, AppVersionId versionId, AppLicenseSecret? licenseSecret, AppPassword? password, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		LogInfo("Executing GetAppRemoteContentInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		AppLicenseSecret? appLicenseSecret = licenseSecret;
		LogTrace("licenseSecret = " + (appLicenseSecret.HasValue ? ((string)appLicenseSecret.GetValueOrDefault()) : null));
		AppPassword? appPassword = password;
		LogTrace("password = " + (appPassword.HasValue ? ((string)appPassword.GetValueOrDefault()) : null));
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemotePackageInfo appRemotePackageInfo = Execute(appRemote, versionId, licenseSecret, password, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemotePackageInfo appRemotePackageInfo2 = appRemotePackageInfo;
			LogTrace("result = " + appRemotePackageInfo2.ToString());
			LogInfo("GetAppRemoteContentInfo finished: success.");
			return appRemotePackageInfo;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteContentInfo finished: AppRemoteVersionNotFound.");
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
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteContentInfo finished: InvalidAppPassword.");
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
			LogInfo("GetAppRemoteContentInfo finished: InvalidAppAuthKey.");
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
			LogInfo("GetAppRemoteContentInfo finished: ExpiredAppAuthKey.");
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
			LogInfo("GetAppRemoteContentInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteContentInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteContentInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteContentInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteContentInfo(context.Resolve<IApiConnection>(), context.Resolve<GetAppRemotePackageUnpackPasswordDelegate>(), context.Resolve<GetAppRemotePack1UnpackInfoBDelegate>(), context.Resolve<GetAppRemotePackageSourcesInfoDelegate>(), context.Resolve<GetCountryCodeDelegate>(), context.Resolve<ParseAppRemoteHashDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
