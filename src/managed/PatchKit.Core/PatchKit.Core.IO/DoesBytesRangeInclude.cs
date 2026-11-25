using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class DoesBytesRangeInclude
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(BytesRange range, BytesCount position)
	{
		if ((long)position < range.Start)
		{
			return false;
		}
		if ((long)position > range.End)
		{
			return false;
		}
		return true;
	}

	public DoesBytesRangeInclude(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(BytesRange range, BytesCount position)
	{
		Arg.IsValid(range, "range");
		Arg.IsValid(position, "position");
		LogInfo("Executing DoesBytesRangeInclude...");
		BytesRange bytesRange = range;
		LogTrace("range = " + bytesRange.ToString());
		BytesCount bytesCount = position;
		LogTrace("position = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(range, position);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("DoesBytesRangeInclude finished: success.");
			return result;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesBytesRangeInclude has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DoesBytesRangeInclude failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DoesBytesRangeInclude finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DoesBytesRangeIncludeDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DoesBytesRangeInclude(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
