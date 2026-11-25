using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetParentAppPath
{
	public GetParentPathDelegate GetParentPath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppPath? Execute(AppPath path)
	{
		Path? path2 = GetParentPath(new Path(path));
		if (!path2.HasValue)
		{
			return null;
		}
		Path? path3 = path2;
		return new AppPath(path3.HasValue ? ((string)path3.GetValueOrDefault()) : null);
	}

	public GetParentAppPath(GetParentPathDelegate getParentPath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getParentPath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetParentPath = getParentPath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppPath? DelegateExecute(AppPath path)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing GetParentAppPath...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			AppPath? appPath = Execute(path);
			EndLogRegion(enabled: true);
			AppPath? appPath2 = appPath;
			LogTrace("result = " + (appPath2.HasValue ? ((string)appPath2.GetValueOrDefault()) : null));
			LogInfo("GetParentAppPath finished: success.");
			return appPath;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetParentAppPath has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetParentAppPath failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetParentAppPath finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetParentAppPathDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetParentAppPath(context.Resolve<GetParentPathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
