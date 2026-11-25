using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteStartInfo
{
	public GetAppRemoteVersionMetaInfoDelegate GetAppRemoteVersionMetaInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppStartInfo? Execute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken)
	{
		return GetAppRemoteVersionMetaInfo(appRemote, versionId, cancellationToken, null).StartInfo;
	}

	public GetAppRemoteStartInfo(GetAppRemoteVersionMetaInfoDelegate getAppRemoteVersionMetaInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionMetaInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionMetaInfo = getAppRemoteVersionMetaInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppStartInfo? DelegateExecute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		LogInfo("Executing GetAppRemoteStartInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppStartInfo? appStartInfo = Execute(appRemote, versionId, cancellationToken);
			EndLogRegion(enabled: false);
			AppStartInfo? appStartInfo2 = appStartInfo;
			LogTrace("result = " + appStartInfo2.ToString());
			LogInfo("GetAppRemoteStartInfo finished: success.");
			return appStartInfo;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteStartInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return null;
			}
			return (AppStartInfo?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteStartInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteStartInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteStartInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteStartInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteStartInfo(context.Resolve<GetAppRemoteVersionMetaInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
