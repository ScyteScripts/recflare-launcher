using System;
using System.IO;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class MoveFileOverwrite
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path path, Path newPath)
	{
		try
		{
			bool flag = false;
			do
			{
				try
				{
					if (new DirectoryInfo(newPath).Exists)
					{
						throw new ConflictingDir(newPath);
					}
					if (new FileInfo(newPath).Exists)
					{
						File.Delete(newPath);
					}
					File.Move(path, newPath);
					flag = true;
				}
				catch (DirectoryNotFoundException inner)
				{
					throw new NotExistingFile(path, inner);
				}
				catch (FileNotFoundException inner2)
				{
					throw new NotExistingFile(path, inner2);
				}
				catch (IOException exception)
				{
					LogWarning("Retrying in 1 second...", exception);
					Thread.Sleep(1000);
				}
			}
			while (!flag);
		}
		catch (UnauthorizedAccessException inner3)
		{
			throw new UnauthorizedAccess(inner3);
		}
	}

	public MoveFileOverwrite(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path path, Path newPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(newPath, "newPath");
		LogInfo("Executing MoveFileOverwrite...");
		LogTrace("path = " + path);
		LogTrace("newPath = " + newPath);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(path, newPath);
			EndLogRegion(enabled: true);
			LogInfo("MoveFileOverwrite finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFileOverwrite finished: UnauthorizedAccess.");
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
			LogInfo("MoveFileOverwrite finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			onNotExistingFile(e2);
			return null;
		}
		catch (ConflictingDir e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFileOverwrite finished: ConflictingDir.");
			if (onConflictingDir == null)
			{
				throw;
			}
			onConflictingDir(e3);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("MoveFileOverwrite has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("MoveFileOverwrite failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("MoveFileOverwrite finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public MoveFileOverwriteDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new MoveFileOverwrite(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
