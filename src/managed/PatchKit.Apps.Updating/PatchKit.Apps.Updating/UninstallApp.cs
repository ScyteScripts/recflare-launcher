using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class UninstallApp
{
	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public DeleteAllAppEntriesDelegate DeleteAllAppEntries { get; }

	public DeleteAppMetaDataFileDelegate DeleteAppMetaDataFile { get; }

	public DeleteAppExtraEntriesDelegate DeleteAppExtraEntries { get; }

	public DeleteDirDelegate DeleteDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app)
	{
		if (!DoesAppRootDirExist(app, null))
		{
			throw new AppRootDirDoesNotExist();
		}
		DeleteAppExtraEntries(app, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
		DeleteAllAppEntries(app, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
		DeleteAppMetaDataFile(app, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
		DeleteDir(app.Path, (UnauthorizedAccess e) =>
		{
		}, (NotExistingDir e) =>
		{
		}, (NotEmptyDir e) =>
		{
		});
	}

	public UninstallApp(DoesAppRootDirExistDelegate doesAppRootDirExist, DeleteAllAppEntriesDelegate deleteAllAppEntries, DeleteAppMetaDataFileDelegate deleteAppMetaDataFile, DeleteAppExtraEntriesDelegate deleteAppExtraEntries, DeleteDirDelegate deleteDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(deleteAllAppEntries);
		Assert.IsNotNull(deleteAppMetaDataFile);
		Assert.IsNotNull(deleteAppExtraEntries);
		Assert.IsNotNull(deleteDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppRootDirExist = doesAppRootDirExist;
		DeleteAllAppEntries = deleteAllAppEntries;
		DeleteAppMetaDataFile = deleteAppMetaDataFile;
		DeleteAppExtraEntries = deleteAppExtraEntries;
		DeleteDir = deleteDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, HandleExceptionDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing UninstallApp...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app);
			EndLogRegion(enabled: true);
			LogInfo("UninstallApp finished: success.");
			return null;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("UninstallApp finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			onAppRootDirDoesNotExist(e);
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("UninstallApp finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			onUnauthorizedAccessToAppRootDir(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("UninstallApp has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("UninstallApp failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("UninstallApp finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public UninstallAppDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new UninstallApp(context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<DeleteAllAppEntriesDelegate>(), context.Resolve<DeleteAppMetaDataFileDelegate>(), context.Resolve<DeleteAppExtraEntriesDelegate>(), context.Resolve<DeleteDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
