using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetMergedFilteredAppRemotePackageChunksRange
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<FilteredAppRemotePackageChunksRange> Execute(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges)
	{
		List<FilteredAppRemotePackageChunksRange> list = filteredChunksRanges.ToList();
		for (int i = 0; i < list.Count - 1; i++)
		{
			FilteredAppRemotePackageChunksRange filteredAppRemotePackageChunksRange = list[i];
			FilteredAppRemotePackageChunksRange filteredAppRemotePackageChunksRange2 = list[i + 1];
			bool v = filteredAppRemotePackageChunksRange.StartChunkIndex <= filteredAppRemotePackageChunksRange2.StartChunkIndex;
			Assert.IsTrue(v, "isReallyNextChunksRange");
			if (filteredAppRemotePackageChunksRange2.StartChunkIndex <= filteredAppRemotePackageChunksRange.EndChunkIndex)
			{
				long endChunkIndex = Math.Max(filteredAppRemotePackageChunksRange.EndChunkIndex, filteredAppRemotePackageChunksRange2.EndChunkIndex);
				list[i + 1] = new FilteredAppRemotePackageChunksRange(filteredAppRemotePackageChunksRange.StartChunkIndex, endChunkIndex);
				list.RemoveAt(i);
				i--;
			}
		}
		return list.AsImmutable();
	}

	public GetMergedFilteredAppRemotePackageChunksRange(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<FilteredAppRemotePackageChunksRange> DelegateExecute(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges)
	{
		Arg.IsValid(filteredChunksRanges, "filteredChunksRanges");
		LogInfo("Executing GetMergedFilteredAppRemotePackageChunksRange...");
		ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray = filteredChunksRanges;
		LogTrace("filteredChunksRanges = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray2 = Execute(filteredChunksRanges);
			EndLogRegion(enabled: true);
			immutableArray = immutableArray2;
			LogTrace("result = " + immutableArray.ToString());
			LogInfo("GetMergedFilteredAppRemotePackageChunksRange finished: success.");
			return immutableArray2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetMergedFilteredAppRemotePackageChunksRange has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetMergedFilteredAppRemotePackageChunksRange failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetMergedFilteredAppRemotePackageChunksRange finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetMergedFilteredAppRemotePackageChunksRangeDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetMergedFilteredAppRemotePackageChunksRange(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
