using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class IsAppFileCurrentPlatformExecutable
{
	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public IsFileCurrentPlatformExecutableDelegate IsFileCurrentPlatformExecutable { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(App app, AppPath path)
	{
		Path path2 = GetAppEntryFullPath(app, path);
		return IsFileCurrentPlatformExecutable(path2, null, (NotExistingFile e) =>
		{
			throw new NotExistingAppFile(app, path);
		});
	}

	public IsAppFileCurrentPlatformExecutable(GetAppEntryFullPathDelegate getAppEntryFullPath, IsFileCurrentPlatformExecutableDelegate isFileCurrentPlatformExecutable, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(isFileCurrentPlatformExecutable);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryFullPath = getAppEntryFullPath;
		IsFileCurrentPlatformExecutable = isFileCurrentPlatformExecutable;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingAppFile> onNotExistingAppFile)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing IsAppFileCurrentPlatformExecutable...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(app, path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsAppFileCurrentPlatformExecutable finished: success.");
			return result;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppFileCurrentPlatformExecutable finished: UnauthorizedAccess.");
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
		catch (NotExistingAppFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppFileCurrentPlatformExecutable finished: NotExistingAppFile.");
			if (onNotExistingAppFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingAppFile(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppFileCurrentPlatformExecutable has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsAppFileCurrentPlatformExecutable failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsAppFileCurrentPlatformExecutable finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsAppFileCurrentPlatformExecutableDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsAppFileCurrentPlatformExecutable(context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<IsFileCurrentPlatformExecutableDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
