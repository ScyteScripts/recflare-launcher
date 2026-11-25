using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class FindAppStartExecutablePath
{
	public GetAppFilePathsDelegate GetAppFilePaths { get; }

	public IsAppFileCurrentPlatformExecutableDelegate IsAppFileCurrentPlatformExecutable { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppPath? Execute(App app)
	{
		foreach (AppPath item in GetAppFilePaths(app, null))
		{
			if (IsAppFileCurrentPlatformExecutable(app, item, null, (NotExistingAppFile e) =>
			{
				throw new UnpredictedException(e);
			}))
			{
				return item;
			}
		}
		return null;
	}

	public FindAppStartExecutablePath(GetAppFilePathsDelegate getAppFilePaths, IsAppFileCurrentPlatformExecutableDelegate isAppFileCurrentPlatformExecutable, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppFilePaths);
		Assert.IsNotNull(isAppFileCurrentPlatformExecutable);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppFilePaths = getAppFilePaths;
		IsAppFileCurrentPlatformExecutable = isAppFileCurrentPlatformExecutable;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppPath? DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing FindAppStartExecutablePath...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			AppPath? appPath = Execute(app);
			EndLogRegion(enabled: true);
			AppPath? appPath2 = appPath;
			LogTrace("result = " + (appPath2.HasValue ? ((string)appPath2.GetValueOrDefault()) : null));
			LogInfo("FindAppStartExecutablePath finished: success.");
			return appPath;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("FindAppStartExecutablePath finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppPath?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("FindAppStartExecutablePath has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("FindAppStartExecutablePath failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("FindAppStartExecutablePath finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public FindAppStartExecutablePathDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new FindAppStartExecutablePath(context.Resolve<GetAppFilePathsDelegate>(), context.Resolve<IsAppFileCurrentPlatformExecutableDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
