using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetFilteredAppRemoteChunkNewIndex
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private long? Execute(long chunkIndex, ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges)
	{
		long num = 0L;
		foreach (FilteredAppRemotePackageChunksRange item in filteredChunksRanges)
		{
			if (item.StartChunkIndex <= chunkIndex && item.EndChunkIndex >= chunkIndex)
			{
				long num2 = chunkIndex - item.StartChunkIndex;
				return num + num2;
			}
			long num3 = item.EndChunkIndex - item.StartChunkIndex + 1;
			num += num3;
		}
		return null;
	}

	public GetFilteredAppRemoteChunkNewIndex(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private long? DelegateExecute(long chunkIndex, ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges)
	{
		Arg.IsValid(filteredChunksRanges, "filteredChunksRanges");
		LogInfo("Executing GetFilteredAppRemoteChunkNewIndex...");
		LogTrace("chunkIndex = " + chunkIndex);
		ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray = filteredChunksRanges;
		LogTrace("filteredChunksRanges = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			long? num = Execute(chunkIndex, filteredChunksRanges);
			EndLogRegion(enabled: true);
			long? num2 = num;
			LogTrace("result = " + num2);
			LogInfo("GetFilteredAppRemoteChunkNewIndex finished: success.");
			return num;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredAppRemoteChunkNewIndex has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemoteChunkNewIndex failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemoteChunkNewIndex finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredAppRemoteChunkNewIndexDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredAppRemoteChunkNewIndex(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
