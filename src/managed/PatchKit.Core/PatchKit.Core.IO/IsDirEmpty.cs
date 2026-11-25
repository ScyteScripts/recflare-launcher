using System;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class IsDirEmpty
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(Path path)
	{
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (!directoryInfo.Exists)
			{
				throw new NotExistingDir(path);
			}
			if (directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories).Length != 0)
			{
				return false;
			}
			return true;
		}
		catch (UnauthorizedAccessException inner)
		{
			throw new UnauthorizedAccess(inner);
		}
	}

	public IsDirEmpty(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingDir> onNotExistingDir)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing IsDirEmpty...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsDirEmpty finished: success.");
			return result;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsDirEmpty finished: UnauthorizedAccess.");
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
		catch (NotExistingDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsDirEmpty finished: NotExistingDir.");
			if (onNotExistingDir == null)
			{
				throw;
			}
			object obj2 = onNotExistingDir(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsDirEmpty has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsDirEmpty failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsDirEmpty finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsDirEmptyDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsDirEmpty(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
