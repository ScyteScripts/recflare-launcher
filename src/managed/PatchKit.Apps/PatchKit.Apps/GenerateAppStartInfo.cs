using System;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GenerateAppStartInfo
{
	public GenerateAppStartExecutableArgsDelegate GenerateAppStartExecutableArgs { get; }

	public FindAppStartExecutablePathDelegate FindAppStartExecutablePath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppStartInfo? Execute(App app)
	{
		AppPath? appPath = FindAppStartExecutablePath(app, null);
		if (appPath.HasValue)
		{
			Text? executableArgs = GenerateAppStartExecutableArgs(app, null);
			return new AppStartInfo(appPath.Value, executableArgs);
		}
		return null;
	}

	public GenerateAppStartInfo(GenerateAppStartExecutableArgsDelegate generateAppStartExecutableArgs, FindAppStartExecutablePathDelegate findAppStartExecutablePath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(generateAppStartExecutableArgs);
		Assert.IsNotNull(findAppStartExecutablePath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GenerateAppStartExecutableArgs = generateAppStartExecutableArgs;
		FindAppStartExecutablePath = findAppStartExecutablePath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppStartInfo? DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GenerateAppStartInfo...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			AppStartInfo? appStartInfo = Execute(app);
			EndLogRegion(enabled: true);
			AppStartInfo? appStartInfo2 = appStartInfo;
			LogTrace("result = " + appStartInfo2.ToString());
			LogInfo("GenerateAppStartInfo finished: success.");
			return appStartInfo;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GenerateAppStartInfo finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppStartInfo?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GenerateAppStartInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GenerateAppStartInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GenerateAppStartInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GenerateAppStartInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GenerateAppStartInfo(context.Resolve<GenerateAppStartExecutableArgsDelegate>(), context.Resolve<FindAppStartExecutablePathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
