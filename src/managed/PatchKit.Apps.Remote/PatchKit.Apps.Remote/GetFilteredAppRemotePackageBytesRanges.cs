using System;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredAppRemotePackageBytesRanges
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<BytesRange> Execute(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, BytesCount chunkSize)
	{
		return filteredChunksRanges.Select((FilteredAppRemotePackageChunksRange chunksRange) => new BytesRange(chunksRange.StartChunkIndex * (long)chunkSize, (chunksRange.EndChunkIndex + 1) * (long)chunkSize - 1)).AsImmutable();
	}

	public GetFilteredAppRemotePackageBytesRanges(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<BytesRange> DelegateExecute(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, BytesCount chunkSize)
	{
		Arg.IsValid(filteredChunksRanges, "filteredChunksRanges");
		Arg.IsValid(chunkSize, "chunkSize");
		LogInfo("Executing GetFilteredAppRemotePackageBytesRanges...");
		ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray = filteredChunksRanges;
		LogTrace("filteredChunksRanges = " + immutableArray.ToString());
		BytesCount bytesCount = chunkSize;
		LogTrace("chunkSize = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<BytesRange> immutableArray2 = Execute(filteredChunksRanges, chunkSize);
			EndLogRegion(enabled: true);
			ImmutableArray<BytesRange> immutableArray3 = immutableArray2;
			LogTrace("result = " + immutableArray3.ToString());
			LogInfo("GetFilteredAppRemotePackageBytesRanges finished: success.");
			return immutableArray2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredAppRemotePackageBytesRanges has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePackageBytesRanges failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePackageBytesRanges finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredAppRemotePackageBytesRangesDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredAppRemotePackageBytesRanges(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
