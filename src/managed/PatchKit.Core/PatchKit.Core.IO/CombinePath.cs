using System;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class CombinePath
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Path Execute(Path first, Path second)
	{
		string text = System.IO.Path.Combine(first, second);
		text = text.Replace('\\', '/');
		text = text.TrimEnd(new char[1] { '/' });
		return new Path(text);
	}

	public CombinePath(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Path DelegateExecute(Path first, Path second)
	{
		Arg.IsValid(first, "first");
		Arg.IsValid(second, "second");
		LogInfo("Executing CombinePath...");
		LogTrace("first = " + first);
		LogTrace("second = " + second);
		try
		{
			BeginLogRegion(enabled: true);
			Path path = Execute(first, second);
			EndLogRegion(enabled: true);
			LogTrace("result = " + path);
			LogInfo("CombinePath finished: success.");
			return path;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("CombinePath has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CombinePath failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CombinePath finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CombinePathDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CombinePath(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
