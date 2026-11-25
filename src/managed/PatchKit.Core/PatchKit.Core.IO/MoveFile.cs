using System;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class MoveFile
{
	public CreateParentDirDelegate CreateParentDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path path, Path newPath)
	{
		try
		{
			if (new FileInfo(newPath).Exists)
			{
				throw new ConflictingFile(newPath);
			}
			if (new DirectoryInfo(newPath).Exists)
			{
				throw new ConflictingDir(newPath);
			}
			CreateParentDir(newPath, null, null);
			File.Move(path, newPath);
		}
		catch (DirectoryNotFoundException inner)
		{
			throw new NotExistingFile(path, inner);
		}
		catch (FileNotFoundException inner2)
		{
			throw new NotExistingFile(path, inner2);
		}
		catch (UnauthorizedAccessException inner3)
		{
			throw new UnauthorizedAccess(inner3);
		}
	}

	public MoveFile(CreateParentDirDelegate createParentDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createParentDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateParentDir = createParentDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path path, Path newPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(newPath, "newPath");
		LogInfo("Executing MoveFile...");
		LogTrace("path = " + path);
		LogTrace("newPath = " + newPath);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(path, newPath);
			EndLogRegion(enabled: true);
			LogInfo("MoveFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFile finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			onNotExistingFile(e2);
			return null;
		}
		catch (ConflictingFile e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFile finished: ConflictingFile.");
			if (onConflictingFile == null)
			{
				throw;
			}
			onConflictingFile(e3);
			return null;
		}
		catch (ConflictingDir e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFile finished: ConflictingDir.");
			if (onConflictingDir == null)
			{
				throw;
			}
			onConflictingDir(e4);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("MoveFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("MoveFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public MoveFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new MoveFile(context.Resolve<CreateParentDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
