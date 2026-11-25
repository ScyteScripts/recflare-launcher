using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class MoveApp
{
	public CreateDirDelegate CreateDir { get; }

	public MoveDirDelegate MoveDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, Path appNewRootDirPath)
	{
		CreateDir(appNewRootDirPath, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppNewRootDir(e);
		}, (ConflictingFile e) =>
		{
			throw new FileIsConflictingWithAppNewRootDir(e);
		});
		try
		{
			MoveDir(app.Path, appNewRootDirPath, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppRootDir();
			}, null, (ConflictingFile e) =>
			{
				throw new UnpredictedException(e);
			}, (NotEmptyDir e) =>
			{
				throw new AppNewRootDirIsNotEmpty();
			});
		}
		catch (NotExistingDir)
		{
		}
	}

	public MoveApp(CreateDirDelegate createDir, MoveDirDelegate moveDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createDir);
		Assert.IsNotNull(moveDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateDir = createDir;
		MoveDir = moveDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, Path appNewRootDirPath, HandleExceptionDelegate<AppNewRootDirIsNotEmpty> onAppNewRootDirIsNotEmpty, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir, HandleExceptionDelegate<UnauthorizedAccessToAppNewRootDir> onUnauthorizedAccessToAppNewRootDir, HandleExceptionDelegate<FileIsConflictingWithAppNewRootDir> onFileIsConflictingWithAppNewRootDir, HandleExceptionDelegate<OutOfAppNewRootDirFreeSpace> onOutOfAppNewRootDirFreeSpace)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(appNewRootDirPath, "appNewRootDirPath");
		LogInfo("Executing MoveApp...");
		LogTrace("app = " + app);
		LogTrace("appNewRootDirPath = " + appNewRootDirPath);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, appNewRootDirPath);
			EndLogRegion(enabled: true);
			LogInfo("MoveApp finished: success.");
			return null;
		}
		catch (AppNewRootDirIsNotEmpty e)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveApp finished: AppNewRootDirIsNotEmpty.");
			if (onAppNewRootDirIsNotEmpty == null)
			{
				throw;
			}
			onAppNewRootDirIsNotEmpty(e);
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveApp finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			onUnauthorizedAccessToAppRootDir(e2);
			return null;
		}
		catch (UnauthorizedAccessToAppNewRootDir e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveApp finished: UnauthorizedAccessToAppNewRootDir.");
			if (onUnauthorizedAccessToAppNewRootDir == null)
			{
				throw;
			}
			onUnauthorizedAccessToAppNewRootDir(e3);
			return null;
		}
		catch (FileIsConflictingWithAppNewRootDir e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveApp finished: FileIsConflictingWithAppNewRootDir.");
			if (onFileIsConflictingWithAppNewRootDir == null)
			{
				throw;
			}
			onFileIsConflictingWithAppNewRootDir(e4);
			return null;
		}
		catch (OutOfAppNewRootDirFreeSpace e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveApp finished: OutOfAppNewRootDirFreeSpace.");
			if (onOutOfAppNewRootDirFreeSpace == null)
			{
				throw;
			}
			onOutOfAppNewRootDirFreeSpace(e5);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveApp has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("MoveApp failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("MoveApp finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public MoveAppDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new MoveApp(context.Resolve<CreateDirDelegate>(), context.Resolve<MoveDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
