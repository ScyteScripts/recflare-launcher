using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class DeleteAppDir
{
	public DoesAppDirExistDelegate DoesAppDirExist { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public DeleteDirDelegate DeleteDir { get; }

	public DeleteAppMetaDataEntryDelegate DeleteAppMetaDataEntry { get; }

	public CleanUnregisteredAppEmptyParentDirsDelegate CleanUnregisteredAppEmptyParentDirs { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path)
	{
		if (DoesAppDirExist(app, path, null))
		{
			Path path2 = GetAppEntryFullPath(app, path);
			DeleteDir(path2, null, (NotExistingDir e) =>
			{
				throw new UnpredictedException(e);
			}, (NotEmptyDir e) =>
			{
			});
			DeleteAppMetaDataEntry(app, path, null);
			CleanUnregisteredAppEmptyParentDirs(app, path, null);
			return;
		}
		throw new NotExistingAppDir(app, path);
	}

	public DeleteAppDir(DoesAppDirExistDelegate doesAppDirExist, GetAppEntryFullPathDelegate getAppEntryFullPath, DeleteDirDelegate deleteDir, DeleteAppMetaDataEntryDelegate deleteAppMetaDataEntry, CleanUnregisteredAppEmptyParentDirsDelegate cleanUnregisteredAppEmptyParentDirs, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppDirExist);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(deleteDir);
		Assert.IsNotNull(deleteAppMetaDataEntry);
		Assert.IsNotNull(cleanUnregisteredAppEmptyParentDirs);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppDirExist = doesAppDirExist;
		GetAppEntryFullPath = getAppEntryFullPath;
		DeleteDir = deleteDir;
		DeleteAppMetaDataEntry = deleteAppMetaDataEntry;
		CleanUnregisteredAppEmptyParentDirs = cleanUnregisteredAppEmptyParentDirs;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingAppDir> onNotExistingAppDir)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing DeleteAppDir...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path);
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppDir finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppDir finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (NotExistingAppDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppDir finished: NotExistingAppDir.");
			if (onNotExistingAppDir == null)
			{
				throw;
			}
			onNotExistingAppDir(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppDir has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppDir failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppDir finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeleteAppDirDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeleteAppDir(context.Resolve<DoesAppDirExistDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<DeleteDirDelegate>(), context.Resolve<DeleteAppMetaDataEntryDelegate>(), context.Resolve<CleanUnregisteredAppEmptyParentDirsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
