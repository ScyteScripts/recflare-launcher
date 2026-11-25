using System;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePack1FilesUnpackInfo
{
	public GetFilteredNewAppRemotePack1FileUnpackInfoDelegate GetFilteredNewAppRemotePack1FileUnpackInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppRemotePack1FileUnpackInfo> Execute(ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> filteredFilesUnpackInfo, ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, BytesCount chunkSize)
	{
		return filteredFilesUnpackInfo.Select((FilteredAppRemotePack1FileUnpackInfo filteredFileUnpackInfo) => GetFilteredNewAppRemotePack1FileUnpackInfo(filteredFileUnpackInfo, filteredChunksRanges, chunkSize)).AsImmutable();
	}

	public GetFilteredNewAppRemotePack1FilesUnpackInfo(GetFilteredNewAppRemotePack1FileUnpackInfoDelegate getFilteredNewAppRemotePack1FileUnpackInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredNewAppRemotePack1FileUnpackInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredNewAppRemotePack1FileUnpackInfo = getFilteredNewAppRemotePack1FileUnpackInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppRemotePack1FileUnpackInfo> DelegateExecute(ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> filteredFilesUnpackInfo, ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, BytesCount chunkSize)
	{
		Arg.IsValid(filteredFilesUnpackInfo, "filteredFilesUnpackInfo");
		Arg.IsValid(filteredChunksRanges, "filteredChunksRanges");
		Arg.IsValid(chunkSize, "chunkSize");
		LogInfo("Executing GetFilteredNewAppRemotePack1FilesUnpackInfo...");
		ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> immutableArray = filteredFilesUnpackInfo;
		LogTrace("filteredFilesUnpackInfo = " + immutableArray.ToString());
		ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray2 = filteredChunksRanges;
		LogTrace("filteredChunksRanges = " + immutableArray2.ToString());
		BytesCount bytesCount = chunkSize;
		LogTrace("chunkSize = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<AppRemotePack1FileUnpackInfo> immutableArray3 = Execute(filteredFilesUnpackInfo, filteredChunksRanges, chunkSize);
			EndLogRegion(enabled: true);
			ImmutableArray<AppRemotePack1FileUnpackInfo> immutableArray4 = immutableArray3;
			LogTrace("result = " + immutableArray4.ToString());
			LogInfo("GetFilteredNewAppRemotePack1FilesUnpackInfo finished: success.");
			return immutableArray3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePack1FilesUnpackInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePack1FilesUnpackInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePack1FilesUnpackInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePack1FilesUnpackInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePack1FilesUnpackInfo(context.Resolve<GetFilteredNewAppRemotePack1FileUnpackInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
