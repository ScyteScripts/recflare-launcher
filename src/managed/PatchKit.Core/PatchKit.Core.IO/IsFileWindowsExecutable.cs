using System;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class IsFileWindowsExecutable
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(Path path)
	{
		try
		{
			if (!new FileInfo(path).Exists)
			{
				throw new NotExistingFile(path);
			}
			return path.Value.EndsWith(".exe");
		}
		catch (UnauthorizedAccessException inner)
		{
			throw new UnauthorizedAccess(inner);
		}
	}

	public IsFileWindowsExecutable(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing IsFileWindowsExecutable...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsFileWindowsExecutable finished: success.");
			return result;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileWindowsExecutable finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileWindowsExecutable finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingFile(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileWindowsExecutable has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileWindowsExecutable failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileWindowsExecutable finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsFileWindowsExecutableDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsFileWindowsExecutable(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
