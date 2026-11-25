using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePack1FileUnpackInfo
{
	public GetFilteredAppRemoteChunkNewIndexDelegate GetFilteredAppRemoteChunkNewIndex { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePack1FileUnpackInfo Execute(FilteredAppRemotePack1FileUnpackInfo filteredFileUnpackInfo, ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, BytesCount chunkSize)
	{
		long? num = GetFilteredAppRemoteChunkNewIndex(filteredFileUnpackInfo.ChunksRange.StartChunkIndex, filteredChunksRanges);
		Assert.IsNotNull(num);
		long v = (num * (long)chunkSize).Value + (long)filteredFileUnpackInfo.StartChunkOffset;
		return new AppRemotePack1FileUnpackInfo(filteredFileUnpackInfo.Path, new BytesCount(v), filteredFileUnpackInfo.Size);
	}

	public GetFilteredNewAppRemotePack1FileUnpackInfo(GetFilteredAppRemoteChunkNewIndexDelegate getFilteredAppRemoteChunkNewIndex, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredAppRemoteChunkNewIndex);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredAppRemoteChunkNewIndex = getFilteredAppRemoteChunkNewIndex;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePack1FileUnpackInfo DelegateExecute(FilteredAppRemotePack1FileUnpackInfo filteredFileUnpackInfo, ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, BytesCount chunkSize)
	{
		Arg.IsValid(filteredFileUnpackInfo, "filteredFileUnpackInfo");
		Arg.IsValid(filteredChunksRanges, "filteredChunksRanges");
		Arg.IsValid(chunkSize, "chunkSize");
		LogInfo("Executing GetFilteredNewAppRemotePack1FileUnpackInfo...");
		FilteredAppRemotePack1FileUnpackInfo filteredAppRemotePack1FileUnpackInfo = filteredFileUnpackInfo;
		LogTrace("filteredFileUnpackInfo = " + filteredAppRemotePack1FileUnpackInfo.ToString());
		ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray = filteredChunksRanges;
		LogTrace("filteredChunksRanges = " + immutableArray.ToString());
		BytesCount bytesCount = chunkSize;
		LogTrace("chunkSize = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePack1FileUnpackInfo appRemotePack1FileUnpackInfo = Execute(filteredFileUnpackInfo, filteredChunksRanges, chunkSize);
			EndLogRegion(enabled: true);
			AppRemotePack1FileUnpackInfo appRemotePack1FileUnpackInfo2 = appRemotePack1FileUnpackInfo;
			LogTrace("result = " + appRemotePack1FileUnpackInfo2.ToString());
			LogInfo("GetFilteredNewAppRemotePack1FileUnpackInfo finished: success.");
			return appRemotePack1FileUnpackInfo;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePack1FileUnpackInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePack1FileUnpackInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePack1FileUnpackInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePack1FileUnpackInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePack1FileUnpackInfo(context.Resolve<GetFilteredAppRemoteChunkNewIndexDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
