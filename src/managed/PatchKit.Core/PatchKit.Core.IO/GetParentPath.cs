using System;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class GetParentPath
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Path? Execute(Path path)
	{
		string directoryName = System.IO.Path.GetDirectoryName(path);
		if (string.IsNullOrEmpty(directoryName))
		{
			return null;
		}
		directoryName = directoryName.Replace('\\', '/');
		directoryName = directoryName.TrimEnd(new char[1] { '/' });
		return new Path(directoryName);
	}

	public GetParentPath(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Path? DelegateExecute(Path path)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing GetParentPath...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Path? path2 = Execute(path);
			EndLogRegion(enabled: true);
			Path? path3 = path2;
			LogTrace("result = " + (path3.HasValue ? ((string)path3.GetValueOrDefault()) : null));
			LogInfo("GetParentPath finished: success.");
			return path2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetParentPath has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetParentPath failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetParentPath finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetParentPathDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetParentPath(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
