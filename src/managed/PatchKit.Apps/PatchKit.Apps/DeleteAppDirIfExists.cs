using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class DeleteAppDirIfExists
{
	public DeleteAppDirDelegate DeleteAppDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path)
	{
		DeleteAppDir(app, path, null, (NotExistingAppDir _) =>
		{
		});
	}

	public DeleteAppDirIfExists(DeleteAppDirDelegate deleteAppDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(deleteAppDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DeleteAppDir = deleteAppDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing DeleteAppDirIfExists...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path);
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppDirIfExists finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppDirIfExists finished: UnauthorizedAccess.");
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
			LogInfo("DeleteAppDirIfExists has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppDirIfExists failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppDirIfExists finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeleteAppDirIfExistsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeleteAppDirIfExists(context.Resolve<DeleteAppDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
