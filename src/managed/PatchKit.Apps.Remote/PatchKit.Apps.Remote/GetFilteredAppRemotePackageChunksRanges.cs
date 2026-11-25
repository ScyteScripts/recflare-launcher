using System;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetFilteredAppRemotePackageChunksRanges
{
	public GetMergedFilteredAppRemotePackageChunksRangeDelegate GetMergedFilteredAppRemotePackageChunksRange { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<FilteredAppRemotePackageChunksRange> Execute(ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> filteredFilesUnpackInfo)
	{
		ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges = filteredFilesUnpackInfo.Select((FilteredAppRemotePack1FileUnpackInfo x) => x.ChunksRange).AsImmutable();
		return GetMergedFilteredAppRemotePackageChunksRange(filteredChunksRanges);
	}

	public GetFilteredAppRemotePackageChunksRanges(GetMergedFilteredAppRemotePackageChunksRangeDelegate getMergedFilteredAppRemotePackageChunksRange, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getMergedFilteredAppRemotePackageChunksRange);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetMergedFilteredAppRemotePackageChunksRange = getMergedFilteredAppRemotePackageChunksRange;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<FilteredAppRemotePackageChunksRange> DelegateExecute(ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> filteredFilesUnpackInfo)
	{
		Arg.IsValid(filteredFilesUnpackInfo, "filteredFilesUnpackInfo");
		LogInfo("Executing GetFilteredAppRemotePackageChunksRanges...");
		ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> immutableArray = filteredFilesUnpackInfo;
		LogTrace("filteredFilesUnpackInfo = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray2 = Execute(filteredFilesUnpackInfo);
			EndLogRegion(enabled: true);
			ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray3 = immutableArray2;
			LogTrace("result = " + immutableArray3.ToString());
			LogInfo("GetFilteredAppRemotePackageChunksRanges finished: success.");
			return immutableArray2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredAppRemotePackageChunksRanges has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePackageChunksRanges failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePackageChunksRanges finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredAppRemotePackageChunksRangesDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredAppRemotePackageChunksRanges(context.Resolve<GetMergedFilteredAppRemotePackageChunksRangeDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
