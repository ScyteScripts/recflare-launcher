using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteLatestVersionId
{
	public GetAppRemoteInfoDelegate GetAppRemoteInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppVersionId Execute(AppRemote appRemote, CancellationToken cancellationToken)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PK_PATCHER_FORCE_VERSION");
		if (environmentVariable != null && int.TryParse(environmentVariable, out var result))
		{
			return new AppVersionId(result);
		}
		return GetAppRemoteInfo(appRemote, cancellationToken, null).LatestVersionId;
	}

	public GetAppRemoteLatestVersionId(GetAppRemoteInfoDelegate getAppRemoteInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteInfo = getAppRemoteInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppVersionId DelegateExecute(AppRemote appRemote, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		LogInfo("Executing GetAppRemoteLatestVersionId...");
		LogTrace("appRemote = " + appRemote);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppVersionId appVersionId = Execute(appRemote, cancellationToken);
			EndLogRegion(enabled: false);
			AppVersionId appVersionId2 = appVersionId;
			LogTrace("result = " + appVersionId2.ToString());
			LogInfo("GetAppRemoteLatestVersionId finished: success.");
			return appVersionId;
		}
		catch (AppRemoteNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteLatestVersionId finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (AppVersionId)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteLatestVersionId has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteLatestVersionId failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteLatestVersionId finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteLatestVersionIdDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteLatestVersionId(context.Resolve<GetAppRemoteInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
