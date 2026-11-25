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
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteVersionEntriesInfo
{
	public IApiConnection ApiConnection { get; }

	public ParseAppRemoteHashDelegate ParseAppRemoteHash { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppRemoteVersionEntryInfo> Execute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken)
	{
		Dictionary<AppVersionId, ImmutableArray<AppRemoteVersionEntryInfo>> versionEntriesInfoCache = appRemote.VersionEntriesInfoCache;
		if (versionEntriesInfoCache.ContainsKey(versionId))
		{
			return versionEntriesInfoCache[versionId];
		}
		return versionEntriesInfoCache[versionId] = GetRawValue(appRemote.Secret, versionId, cancellationToken).Files.Select(GetEntryInfo).AsImmutable();
	}

	private AppRemoteVersionEntryInfo GetEntryInfo(AppContentSummaryFile raw)
	{
		Assert.IsNotNull(raw.Path);
		AppPath path = new AppPath(raw.Path);
		if (raw.Path.EndsWith("/"))
		{
			return new AppRemoteVersionDirInfo(path);
		}
		string hash = raw.Hash;
		Assert.IsNotNull(hash);
		Hash hash2 = ParseAppRemoteHash(new Text(hash));
		return new AppRemoteVersionFileInfo(path, hash2, new BytesCount(raw.Size));
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

	public GetAppRemoteVersionEntriesInfo(IApiConnection apiConnection, ParseAppRemoteHashDelegate parseAppRemoteHash, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(apiConnection);
		Assert.IsNotNull(parseAppRemoteHash);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ApiConnection = apiConnection;
		ParseAppRemoteHash = parseAppRemoteHash;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppRemoteVersionEntryInfo> DelegateExecute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		LogInfo("Executing GetAppRemoteVersionEntriesInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			ImmutableArray<AppRemoteVersionEntryInfo> immutableArray = Execute(appRemote, versionId, cancellationToken);
			EndLogRegion(enabled: false);
			ImmutableArray<AppRemoteVersionEntryInfo> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("GetAppRemoteVersionEntriesInfo finished: success.");
			return immutableArray;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionEntriesInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (ImmutableArray<AppRemoteVersionEntryInfo>)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionEntriesInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionEntriesInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionEntriesInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteVersionEntriesInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteVersionEntriesInfo(context.Resolve<IApiConnection>(), context.Resolve<ParseAppRemoteHashDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
