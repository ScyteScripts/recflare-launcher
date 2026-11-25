using System;
using System.IO;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class MoveDir
{
	public DoesFileExistDelegate DoesFileExist { get; }

	public DoesDirExistDelegate DoesDirExist { get; }

	public IsDirEmptyDelegate IsDirEmpty { get; }

	public DeleteDirDelegate DeleteDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path path, Path newPath)
	{
		bool flag = false;
		do
		{
			try
			{
				if (!DoesDirExist(path, null))
				{
					throw new NotExistingDir(path);
				}
				if (DoesFileExist(newPath, null))
				{
					throw new ConflictingFile(newPath);
				}
				if (DoesDirExist(newPath, null))
				{
					if (!IsDirEmpty(newPath, null, (NotExistingDir e) =>
					{
						throw new UnpredictedException(e);
					}))
					{
						throw new NotEmptyDir(newPath);
					}
					DeleteDir(newPath, null, (NotExistingDir e) =>
					{
						throw new UnpredictedException(e);
					}, (NotEmptyDir e) =>
					{
						throw new UnpredictedException(e);
					});
				}
				Directory.Move(path, newPath);
				flag = true;
			}
			catch (UnauthorizedAccessException inner)
			{
				throw new UnauthorizedAccess(inner);
			}
			catch (IOException exception)
			{
				LogWarning("Retrying in 1 second...", exception);
				Thread.Sleep(1000);
			}
		}
		while (!flag);
	}

	public MoveDir(DoesFileExistDelegate doesFileExist, DoesDirExistDelegate doesDirExist, IsDirEmptyDelegate isDirEmpty, DeleteDirDelegate deleteDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesFileExist);
		Assert.IsNotNull(doesDirExist);
		Assert.IsNotNull(isDirEmpty);
		Assert.IsNotNull(deleteDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesFileExist = doesFileExist;
		DoesDirExist = doesDirExist;
		IsDirEmpty = isDirEmpty;
		DeleteDir = deleteDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path path, Path newPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingDir> onNotExistingDir, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<NotEmptyDir> onNotEmptyDir)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(newPath, "newPath");
		LogInfo("Executing MoveDir...");
		LogTrace("path = " + path);
		LogTrace("newPath = " + newPath);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(path, newPath);
			EndLogRegion(enabled: true);
			LogInfo("MoveDir finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveDir finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (NotExistingDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveDir finished: NotExistingDir.");
			if (onNotExistingDir == null)
			{
				throw;
			}
			onNotExistingDir(e2);
			return null;
		}
		catch (ConflictingFile e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveDir finished: ConflictingFile.");
			if (onConflictingFile == null)
			{
				throw;
			}
			onConflictingFile(e3);
			return null;
		}
		catch (NotEmptyDir e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveDir finished: NotEmptyDir.");
			if (onNotEmptyDir == null)
			{
				throw;
			}
			onNotEmptyDir(e4);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveDir has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("MoveDir failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("MoveDir finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public MoveDirDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new MoveDir(context.Resolve<DoesFileExistDelegate>(), context.Resolve<DoesDirExistDelegate>(), context.Resolve<IsDirEmptyDelegate>(), context.Resolve<DeleteDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
