using System;
using System.IO;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class WriteTextToFile
{
	public CreateParentDirDelegate CreateParentDir { get; }

	public CreateFileDelegate CreateFile { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path path, Text text)
	{
		try
		{
			bool flag = false;
			do
			{
				try
				{
					if (new FileInfo(path).Exists)
					{
						throw new ConflictingFile(path);
					}
					if (new DirectoryInfo(path).Exists)
					{
						throw new ConflictingDir(path);
					}
					CreateParentDir(path, null, (ConflictingFile e) =>
					{
						throw new UnpredictedException(e);
					});
					File.WriteAllText(path, text);
					flag = true;
				}
				catch (IOException exception)
				{
					LogWarning("Retrying in 1 second...", exception);
					Thread.Sleep(1000);
				}
			}
			while (!flag);
		}
		catch (UnauthorizedAccessException inner)
		{
			throw new UnauthorizedAccess(inner);
		}
	}

	public WriteTextToFile(CreateParentDirDelegate createParentDir, CreateFileDelegate createFile, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createParentDir);
		Assert.IsNotNull(createFile);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateParentDir = createParentDir;
		CreateFile = createFile;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path path, Text text, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingDir> onConflictingDir, HandleExceptionDelegate<ConflictingFile> onConflictingFile)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(text, "text");
		LogInfo("Executing WriteTextToFile...");
		LogTrace("path = " + path);
		LogTrace("text = " + text);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(path, text);
			EndLogRegion(enabled: true);
			LogInfo("WriteTextToFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("WriteTextToFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (ConflictingDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("WriteTextToFile finished: ConflictingDir.");
			if (onConflictingDir == null)
			{
				throw;
			}
			onConflictingDir(e2);
			return null;
		}
		catch (ConflictingFile e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("WriteTextToFile finished: ConflictingFile.");
			if (onConflictingFile == null)
			{
				throw;
			}
			onConflictingFile(e3);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("WriteTextToFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("WriteTextToFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("WriteTextToFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public WriteTextToFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new WriteTextToFile(context.Resolve<CreateParentDirDelegate>(), context.Resolve<CreateFileDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
