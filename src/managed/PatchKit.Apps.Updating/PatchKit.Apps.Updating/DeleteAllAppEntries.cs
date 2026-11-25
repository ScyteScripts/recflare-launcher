using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class DeleteAllAppEntries
{
	public GetAppEntryPathsDelegate GetAppEntryPaths { get; }

	public DeleteAppFileOrDirDelegate DeleteAppFileOrDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app)
	{
		foreach (AppPath item in GetAppEntryPaths(app, null))
		{
			DeleteAppFileOrDir(app, item, null);
		}
	}

	public DeleteAllAppEntries(GetAppEntryPathsDelegate getAppEntryPaths, DeleteAppFileOrDirDelegate deleteAppFileOrDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryPaths);
		Assert.IsNotNull(deleteAppFileOrDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryPaths = getAppEntryPaths;
		DeleteAppFileOrDir = deleteAppFileOrDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing DeleteAllAppEntries...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app);
			EndLogRegion(enabled: true);
			LogInfo("DeleteAllAppEntries finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAllAppEntries finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAllAppEntries has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAllAppEntries failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAllAppEntries finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeleteAllAppEntriesDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeleteAllAppEntries(context.Resolve<GetAppEntryPathsDelegate>(), context.Resolve<DeleteAppFileOrDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
