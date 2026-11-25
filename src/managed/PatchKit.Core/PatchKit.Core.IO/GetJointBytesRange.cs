using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class GetJointBytesRange
{
	public DoesBytesRangeIncludeDelegate DoesBytesRangeInclude { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private BytesRange? Execute(BytesRange range, BytesRange other)
	{
		long start;
		if (DoesBytesRangeInclude(range, new BytesCount(other.Start)))
		{
			start = other.Start;
		}
		else
		{
			if (!DoesBytesRangeInclude(other, new BytesCount(range.Start)))
			{
				return null;
			}
			start = range.Start;
		}
		long end = Math.Min(range.End, other.End);
		return new BytesRange(start, end);
	}

	public GetJointBytesRange(DoesBytesRangeIncludeDelegate doesBytesRangeInclude, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesBytesRangeInclude);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesBytesRangeInclude = doesBytesRangeInclude;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private BytesRange? DelegateExecute(BytesRange range, BytesRange other)
	{
		Arg.IsValid(range, "range");
		Arg.IsValid(other, "other");
		LogInfo("Executing GetJointBytesRange...");
		BytesRange bytesRange = range;
		LogTrace("range = " + bytesRange.ToString());
		bytesRange = other;
		LogTrace("other = " + bytesRange.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			BytesRange? bytesRange2 = Execute(range, other);
			EndLogRegion(enabled: true);
			BytesRange? bytesRange3 = bytesRange2;
			LogTrace("result = " + bytesRange3.ToString());
			LogInfo("GetJointBytesRange finished: success.");
			return bytesRange2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetJointBytesRange has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetJointBytesRange failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetJointBytesRange finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetJointBytesRangeDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetJointBytesRange(context.Resolve<DoesBytesRangeIncludeDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
