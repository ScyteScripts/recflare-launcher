using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Api;
using PatchKit.Api.Models;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteVersionMetaInfo
{
	public IApiConnection ApiConnection { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemoteVersionMetaInfo Execute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken)
	{
		Dictionary<AppVersionId, AppRemoteVersionMetaInfo> versionMetaInfoCache = appRemote.VersionMetaInfoCache;
		if (versionMetaInfoCache.ContainsKey(versionId))
		{
			return versionMetaInfoCache[versionId];
		}
		AppVersion rawValue = GetRawValue(appRemote.Secret, versionId, cancellationToken);
		Assert.IsNotNull(rawValue.Label);
		AppRemoteVersionMetaInfo result = (versionMetaInfoCache[versionId] = new AppRemoteVersionMetaInfo(new Text(rawValue.Label), (rawValue.Changelog != null) ? new Text?(new Text(rawValue.Changelog)) : ((Text?)null), GetAppStartInfo(rawValue)));
		return result;
	}

	private AppStartInfo? GetAppStartInfo(AppVersion raw)
	{
		if (string.IsNullOrWhiteSpace(raw.MainExecutable))
		{
			return null;
		}
		AppPath executablePath = new AppPath(raw.MainExecutable);
		Text? executableArgs = null;
		if (!string.IsNullOrEmpty(raw.MainExecutableArgs))
		{
			executableArgs = new Text(raw.MainExecutableArgs);
		}
		return new AppStartInfo(executablePath, executableArgs);
	}

	private AppVersion GetRawValue(AppSecret secret, AppVersionId versionId, CancellationToken cancellationToken)
	{
		AppVersion? appVersion = null;
		while (!appVersion.HasValue)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				appVersion = ApiConnection.GetAppVersion(secret, versionId, null, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken);
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
		return appVersion.Value;
	}

	public GetAppRemoteVersionMetaInfo(IApiConnection apiConnection, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(apiConnection);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ApiConnection = apiConnection;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemoteVersionMetaInfo DelegateExecute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		LogInfo("Executing GetAppRemoteVersionMetaInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemoteVersionMetaInfo appRemoteVersionMetaInfo = Execute(appRemote, versionId, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemoteVersionMetaInfo appRemoteVersionMetaInfo2 = appRemoteVersionMetaInfo;
			LogTrace("result = " + appRemoteVersionMetaInfo2.ToString());
			LogInfo("GetAppRemoteVersionMetaInfo finished: success.");
			return appRemoteVersionMetaInfo;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionMetaInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (AppRemoteVersionMetaInfo)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionMetaInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionMetaInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionMetaInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteVersionMetaInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteVersionMetaInfo(context.Resolve<IApiConnection>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
