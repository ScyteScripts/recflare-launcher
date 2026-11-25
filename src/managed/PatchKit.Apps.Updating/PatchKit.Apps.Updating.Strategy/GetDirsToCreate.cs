using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Strategy;

public class GetDirsToCreate
{
	public GetAppRemoteVersionDirPathsDelegate GetAppRemoteVersionDirPaths { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppPath> Execute(UpdateAppContext context, CancellationToken cancellationToken)
	{
		return GetAppRemoteVersionDirPaths(context.AppRemote, context.TargetVersionId, cancellationToken, null);
	}

	public GetDirsToCreate(GetAppRemoteVersionDirPathsDelegate getAppRemoteVersionDirPaths, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionDirPaths);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionDirPaths = getAppRemoteVersionDirPaths;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppPath> DelegateExecute(UpdateAppContext context, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsValid(context, "context");
		LogInfo("Executing GetDirsToCreate...");
		UpdateAppContext updateAppContext = context;
		LogTrace("context = " + updateAppContext.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<AppPath> immutableArray = Execute(context, cancellationToken);
			EndLogRegion(enabled: true);
			ImmutableArray<AppPath> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("GetDirsToCreate finished: success.");
			return immutableArray;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetDirsToCreate finished: AppRemoteVersionNotFound.");
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
			EndLogRegion(enabled: true);
			LogInfo("GetDirsToCreate has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetDirsToCreate failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetDirsToCreate finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetDirsToCreateDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetDirsToCreate(context.Resolve<GetAppRemoteVersionDirPathsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
