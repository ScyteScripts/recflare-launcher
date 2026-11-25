using System;
using System.Linq;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetAppRemoteVersionDirPaths
{
	public GetAppRemoteVersionDirsInfoDelegate GetAppRemoteVersionDirsInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppPath> Execute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken)
	{
		return (from x in GetAppRemoteVersionDirsInfo(appRemote, versionId, cancellationToken, null)
			select x.Path).AsImmutable();
	}

	public GetAppRemoteVersionDirPaths(GetAppRemoteVersionDirsInfoDelegate getAppRemoteVersionDirsInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionDirsInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionDirsInfo = getAppRemoteVersionDirsInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppPath> DelegateExecute(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(versionId, "versionId");
		LogInfo("Executing GetAppRemoteVersionDirPaths...");
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
			LogInfo("GetAppRemoteVersionDirPaths finished: success.");
			return immutableArray;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemoteVersionDirPaths finished: AppRemoteVersionNotFound.");
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
			LogInfo("GetAppRemoteVersionDirPaths has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionDirPaths failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemoteVersionDirPaths finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemoteVersionDirPathsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemoteVersionDirPaths(context.Resolve<GetAppRemoteVersionDirsInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
