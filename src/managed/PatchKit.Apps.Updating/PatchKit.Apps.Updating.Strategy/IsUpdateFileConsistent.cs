using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Strategy;

public class IsUpdateFileConsistent
{
	public GetAppRemoteVersionFileInfoDelegate GetAppRemoteVersionFileInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(AppRemote appRemote, UpdateFileState state, CancellationToken cancellationToken)
	{
		if (!state.Size.HasValue && !state.Hash.HasValue)
		{
			return true;
		}
		AppRemoteVersionFileInfo? appRemoteVersionFileInfo = GetAppRemoteVersionFileInfo(appRemote, state.VersionId, state.Path, cancellationToken, null);
		if (!appRemoteVersionFileInfo.HasValue)
		{
			return false;
		}
		if (state.Size.HasValue && (long)appRemoteVersionFileInfo.Value.Size != (long)state.Size.Value)
		{
			return false;
		}
		if (state.Hash.HasValue && appRemoteVersionFileInfo.Value.Hash.Value != state.Hash.Value.Value)
		{
			return false;
		}
		return true;
	}

	public IsUpdateFileConsistent(GetAppRemoteVersionFileInfoDelegate getAppRemoteVersionFileInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionFileInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionFileInfo = getAppRemoteVersionFileInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(AppRemote appRemote, UpdateFileState state, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(state, "state");
		LogInfo("Executing IsUpdateFileConsistent...");
		LogTrace("appRemote = " + appRemote);
		UpdateFileState updateFileState = state;
		LogTrace("state = " + updateFileState.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(appRemote, state, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsUpdateFileConsistent finished: success.");
			return result;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsUpdateFileConsistent finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsUpdateFileConsistent has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsUpdateFileConsistent failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsUpdateFileConsistent finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsUpdateFileConsistentDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsUpdateFileConsistent(context.Resolve<GetAppRemoteVersionFileInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
