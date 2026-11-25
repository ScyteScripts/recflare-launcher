using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class DeleteRedundantAppEntries
{
	public DeleteAppFileOrDirDelegate DeleteAppFileOrDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, ImmutableArray<AppPath> entries)
	{
		foreach (AppPath item in entries)
		{
			DeleteAppFileOrDir(app, item, null);
		}
	}

	public DeleteRedundantAppEntries(DeleteAppFileOrDirDelegate deleteAppFileOrDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(deleteAppFileOrDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DeleteAppFileOrDir = deleteAppFileOrDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, ImmutableArray<AppPath> entries, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(entries, "entries");
		LogInfo("Executing DeleteRedundantAppEntries...");
		LogTrace("app = " + app);
		ImmutableArray<AppPath> immutableArray = entries;
		LogTrace("entries = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, entries);
			EndLogRegion(enabled: true);
			LogInfo("DeleteRedundantAppEntries finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteRedundantAppEntries finished: UnauthorizedAccess.");
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
			LogInfo("DeleteRedundantAppEntries has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteRedundantAppEntries failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteRedundantAppEntries finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeleteRedundantAppEntriesDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeleteRedundantAppEntries(context.Resolve<DeleteAppFileOrDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
