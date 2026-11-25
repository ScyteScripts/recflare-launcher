using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteVersionFileInfo
{
	public GetAppRemoteVersionFilesInfoDelegate GetAppRemoteVersionFilesInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemoteVersionFileInfo? Execute(AppRemote appRemote, AppVersionId versionId, AppPath path, CancellationToken cancellationToken)
	{
		ImmutableArray<AppRemoteVersionFileInfo> immutableArray = GetAppRemoteVersionFilesInfo(appRemote, versionId, cancellationToken, null);
		return immutableArray.FirstOrNull((AppRemoteVersionFileInfo info) => (string)info.Path == (string)path);
	}

	public GetAppRemoteVersionFileInfo(GetAppRemoteVersionFilesInfoDelegate getAppRemoteVersionFilesInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionFilesInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionFilesInfo = getAppRemoteVersionFilesInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemoteVersionFileInfo? DelegateExecute(AppRemote appRemote, AppVersionId versionId, AppPath path, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetAppRemoteVersionFileInfo...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		LogTrace("path = " + path);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemoteVersionFileInfo? appRemoteVersionFileInfo = Execute(appRemote, versionId, path, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemoteVersionFileInfo? appRemoteVersionFileInfo2 = appRemoteVersionFileInfo;
			LogTrace("result = " + appRemoteVersionFileInfo2.ToString());
			LogInfo("GetAppRemoteVersionFileInfo finished: success.");
			return appRemoteVersionFileInfo;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionFileInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return null;
			}
			return (AppRemoteVersionFileInfo?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionFileInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionFileInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionFileInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteVersionFileInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteVersionFileInfo(context.Resolve<GetAppRemoteVersionFilesInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
