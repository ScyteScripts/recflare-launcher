using System;
using System.Linq;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteVersionEntryPaths
{
	public GetAppRemoteVersionEntriesInfoDelegate GetAppRemoteVersionEntriesInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppPath> Execute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken)
	{
		return (from x in GetAppRemoteVersionEntriesInfo(appRemote, versionId, cancellationToken, null)
			select x.Path).AsImmutable();
	}

	public GetAppRemoteVersionEntryPaths(GetAppRemoteVersionEntriesInfoDelegate getAppRemoteVersionEntriesInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionEntriesInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionEntriesInfo = getAppRemoteVersionEntriesInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppPath> DelegateExecute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		LogInfo("Executing GetAppRemoteVersionEntryPaths...");
		LogTrace("appRemote = " + appRemote);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			ImmutableArray<AppPath> immutableArray = Execute(appRemote, versionId, cancellationToken);
			EndLogRegion(enabled: false);
			ImmutableArray<AppPath> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("GetAppRemoteVersionEntryPaths finished: success.");
			return immutableArray;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionEntryPaths finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteVersionNotFound(e);
			if (obj == null)
			{
				return default;
			}
			return (ImmutableArray<AppPath>)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionEntryPaths has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionEntryPaths failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionEntryPaths finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteVersionEntryPathsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteVersionEntryPaths(context.Resolve<GetAppRemoteVersionEntriesInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
