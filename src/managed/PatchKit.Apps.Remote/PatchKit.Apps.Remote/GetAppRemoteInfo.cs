using System;
using System.Threading;
using Autofac;
using PatchKit.Api;
using PatchKit.Api.Models;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteInfo
{
	public IApiConnection ApiConnection { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemoteInfo Execute(AppRemote appRemote, CancellationToken cancellationToken)
	{
		if (appRemote.InfoCache.HasValue)
		{
			return appRemote.InfoCache.Value;
		}
		PatchKit.Api.Models.App rawValue = GetRawValue(appRemote.Secret, cancellationToken);
		Assert.IsNotNull(rawValue.PatcherDownloadSpeedUnit);
		AppRemoteInfo appRemoteInfo = new AppRemoteInfo((rawValue.DisplayName != null) ? new Text?(new Text(rawValue.DisplayName)) : ((Text?)null), rawValue.UseKeys, rawValue.UseToken, rawValue.UsePassword, new AppVersionId(rawValue.CurrentVersion), (rawValue.LowestVersionWithDiff > 0) ? new AppVersionId?(new AppVersionId(rawValue.LowestVersionWithDiff)) : ((AppVersionId?)null), new AppRemotePatcherInfo(downloadSpeedUnit: new Text(rawValue.PatcherDownloadSpeedUnit), whitelabel: rawValue.PatcherWhitelabel, bannerInfo: (rawValue.PatcherBannerImage == null) ? ((AppRemotePatcherBannerInfo?)null) : new AppRemotePatcherBannerInfo?(new AppRemotePatcherBannerInfo(new HttpAddress(new Uri(rawValue.PatcherBannerImage)), rawValue.PatcherBannerImageDimensions.X, rawValue.PatcherBannerImageDimensions.Y))));
		appRemote.InfoCache = appRemoteInfo;
		return appRemoteInfo;
	}

	private PatchKit.Api.Models.App GetRawValue(AppSecret secret, CancellationToken cancellationToken)
	{
		PatchKit.Api.Models.App? app = null;
		while (!app.HasValue)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				app = ApiConnection.GetApplicationInfo(secret, new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), cancellationToken);
			}
			catch (ApiConnectionException)
			{
			}
			catch (ApiResponseException ex2)
			{
				if (ex2.StatusCode == 404)
				{
					throw new AppRemoteNotFound(secret);
				}
			}
		}
		return app.Value;
	}

	public GetAppRemoteInfo(IApiConnection apiConnection, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

	private AppRemoteInfo DelegateExecute(AppRemote appRemote, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		LogInfo("Executing GetAppRemoteInfo...");
		LogTrace("appRemote = " + appRemote);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemoteInfo appRemoteInfo = Execute(appRemote, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemoteInfo appRemoteInfo2 = appRemoteInfo;
			LogTrace("result = " + appRemoteInfo2.ToString());
			LogInfo("GetAppRemoteInfo finished: success.");
			return appRemoteInfo;
		}
		catch (AppRemoteNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteInfo finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (AppRemoteInfo)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteInfo(context.Resolve<IApiConnection>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
