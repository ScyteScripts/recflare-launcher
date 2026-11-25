using System;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class CopyFile
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path sourcePath, Path targetPath)
	{
		try
		{
			if (new FileInfo(targetPath).Exists)
			{
				throw new ConflictingFile(targetPath);
			}
			if (new DirectoryInfo(targetPath).Exists)
			{
				throw new ConflictingDir(targetPath);
			}
			File.Copy(sourcePath, targetPath);
		}
		catch (DirectoryNotFoundException inner)
		{
			throw new NotExistingFile(sourcePath, inner);
		}
		catch (FileNotFoundException inner2)
		{
			throw new NotExistingFile(sourcePath, inner2);
		}
		catch (UnauthorizedAccessException inner3)
		{
			throw new UnauthorizedAccess(inner3);
		}
	}

	public CopyFile(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path sourcePath, Path targetPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir)
	{
		Arg.IsValid(sourcePath, "sourcePath");
		Arg.IsValid(targetPath, "targetPath");
		LogInfo("Executing CopyFile...");
		LogTrace("sourcePath = " + sourcePath);
		LogTrace("targetPath = " + targetPath);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(sourcePath, targetPath);
			EndLogRegion(enabled: true);
			LogInfo("CopyFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("CopyFile finished: UnauthorizedAccess.");
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
			LogInfo("CopyFile finished: NotExistingFile.");
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
			LogInfo("CopyFile finished: ConflictingFile.");
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
			LogInfo("CopyFile finished: ConflictingDir.");
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
			LogInfo("CopyFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CopyFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CopyFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CopyFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CopyFile(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
