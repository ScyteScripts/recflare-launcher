using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Strategy;

public class GetEntriesToDelete
{
	public GetAppRemoteVersionEntryPathsDelegate GetAppRemoteVersionEntryPaths { get; }

	public GetAppEntryPathsDelegate GetAppEntryPaths { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppPath> Execute(UpdateAppContext context, CancellationToken cancellationToken)
	{
		ImmutableArray<AppPath> immutableArray = GetAppRemoteVersionEntryPaths(context.AppRemote, context.TargetVersionId, cancellationToken, null);
		List<AppPath> list = GetAppEntryPaths(context.App, null).ToList();
		foreach (AppPath item in immutableArray)
		{
			list.Remove(item);
		}
		return list.AsImmutable();
	}

	public GetEntriesToDelete(GetAppRemoteVersionEntryPathsDelegate getAppRemoteVersionEntryPaths, GetAppEntryPathsDelegate getAppEntryPaths, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionEntryPaths);
		Assert.IsNotNull(getAppEntryPaths);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionEntryPaths = getAppRemoteVersionEntryPaths;
		GetAppEntryPaths = getAppEntryPaths;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppPath> DelegateExecute(UpdateAppContext context, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsValid(context, "context");
		LogInfo("Executing GetEntriesToDelete...");
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
			LogInfo("GetEntriesToDelete finished: success.");
			return immutableArray;
		}
		catch (AppRemoteVersionNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetEntriesToDelete finished: AppRemoteVersionNotFound.");
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
			LogInfo("GetEntriesToDelete has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetEntriesToDelete failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetEntriesToDelete finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetEntriesToDeleteDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetEntriesToDelete(context.Resolve<GetAppRemoteVersionEntryPathsDelegate>(), context.Resolve<GetAppEntryPathsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
