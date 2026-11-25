using System;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePack1Info
{
	public GetFilteredAppRemotePack1FilesUnpackInfoDelegate GetFilteredAppRemotePack1FilesUnpackInfo { get; }

	public GetFilteredNewAppRemotePackageSourcesInfoDelegate GetFilteredNewAppRemotePackageSourcesInfo { get; }

	public GetFilteredAppRemotePackageChunksRangesDelegate GetFilteredAppRemotePackageChunksRanges { get; }

	public GetFilteredAppRemotePackageBytesRangesDelegate GetFilteredAppRemotePackageBytesRanges { get; }

	public GetFilteredNewAppRemotePack1FilesUnpackInfoDelegate GetFilteredNewAppRemotePack1FilesUnpackInfo { get; }

	public GetFilteredNewAppRemotePackageChunksInfoDelegate GetFilteredNewAppRemotePackageChunksInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageInfo Execute(AppRemotePack1UnpackInfo unpackInfo, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo)
	{
		BytesCount chunkSize = (chunksInfo.HasValue ? chunksInfo.Value.Size : new BytesCount(1L));
		ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> filteredFilesUnpackInfo = GetFilteredAppRemotePack1FilesUnpackInfo(unpackInfo.FilesInfo, newFilesInfo, chunkSize);
		ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges = GetFilteredAppRemotePackageChunksRanges(filteredFilesUnpackInfo);
		ImmutableArray<BytesRange> filteredBytesRanges = GetFilteredAppRemotePackageBytesRanges(filteredChunksRanges, chunkSize);
		ImmutableArray<AppRemotePackageSourceInfo> immutableArray = GetFilteredNewAppRemotePackageSourcesInfo(sourcesInfo, filteredBytesRanges);
		ImmutableArray<AppRemotePack1FileUnpackInfo> filesInfo = GetFilteredNewAppRemotePack1FilesUnpackInfo(filteredFilesUnpackInfo, filteredChunksRanges, chunkSize);
		AppRemotePack1UnpackInfo appRemotePack1UnpackInfo = new AppRemotePack1UnpackInfo(filesInfo, unpackInfo.Encryption, unpackInfo.CompressionMethod, unpackInfo.Password);
		AppRemotePackageChunksInfo? chunksInfo2 = GetFilteredNewAppRemotePackageChunksInfo(filteredChunksRanges, chunksInfo);
		long v = immutableArray.First().PartsInfo.Sum((AppRemotePackageSourcePartInfo x) => x.Size);
		return new AppRemotePackageInfo(newFilesInfo, appRemotePack1UnpackInfo, immutableArray, chunksInfo2, new BytesCount(v));
	}

	public GetFilteredNewAppRemotePack1Info(GetFilteredAppRemotePack1FilesUnpackInfoDelegate getFilteredAppRemotePack1FilesUnpackInfo, GetFilteredNewAppRemotePackageSourcesInfoDelegate getFilteredNewAppRemotePackageSourcesInfo, GetFilteredAppRemotePackageChunksRangesDelegate getFilteredAppRemotePackageChunksRanges, GetFilteredAppRemotePackageBytesRangesDelegate getFilteredAppRemotePackageBytesRanges, GetFilteredNewAppRemotePack1FilesUnpackInfoDelegate getFilteredNewAppRemotePack1FilesUnpackInfo, GetFilteredNewAppRemotePackageChunksInfoDelegate getFilteredNewAppRemotePackageChunksInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredAppRemotePack1FilesUnpackInfo);
		Assert.IsNotNull(getFilteredNewAppRemotePackageSourcesInfo);
		Assert.IsNotNull(getFilteredAppRemotePackageChunksRanges);
		Assert.IsNotNull(getFilteredAppRemotePackageBytesRanges);
		Assert.IsNotNull(getFilteredNewAppRemotePack1FilesUnpackInfo);
		Assert.IsNotNull(getFilteredNewAppRemotePackageChunksInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredAppRemotePack1FilesUnpackInfo = getFilteredAppRemotePack1FilesUnpackInfo;
		GetFilteredNewAppRemotePackageSourcesInfo = getFilteredNewAppRemotePackageSourcesInfo;
		GetFilteredAppRemotePackageChunksRanges = getFilteredAppRemotePackageChunksRanges;
		GetFilteredAppRemotePackageBytesRanges = getFilteredAppRemotePackageBytesRanges;
		GetFilteredNewAppRemotePack1FilesUnpackInfo = getFilteredNewAppRemotePack1FilesUnpackInfo;
		GetFilteredNewAppRemotePackageChunksInfo = getFilteredNewAppRemotePackageChunksInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageInfo DelegateExecute(AppRemotePack1UnpackInfo unpackInfo, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo)
	{
		Arg.IsValid(unpackInfo, "unpackInfo");
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(chunksInfo, "chunksInfo");
		Arg.IsValid(size, "size");
		Arg.IsValid(newFilesInfo, "newFilesInfo");
		LogInfo("Executing GetFilteredNewAppRemotePack1Info...");
		AppRemotePack1UnpackInfo appRemotePack1UnpackInfo = unpackInfo;
		LogTrace("unpackInfo = " + appRemotePack1UnpackInfo.ToString());
		ImmutableArray<AppRemotePackageSourceInfo> immutableArray = sourcesInfo;
		LogTrace("sourcesInfo = " + immutableArray.ToString());
		AppRemotePackageChunksInfo? appRemotePackageChunksInfo = chunksInfo;
		LogTrace("chunksInfo = " + appRemotePackageChunksInfo.ToString());
		BytesCount bytesCount = size;
		LogTrace("size = " + bytesCount.ToString());
		ImmutableArray<AppRemotePackageFileInfo> immutableArray2 = newFilesInfo;
		LogTrace("newFilesInfo = " + immutableArray2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo = Execute(unpackInfo, sourcesInfo, chunksInfo, size, newFilesInfo);
			EndLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo2 = appRemotePackageInfo;
			LogTrace("result = " + appRemotePackageInfo2.ToString());
			LogInfo("GetFilteredNewAppRemotePack1Info finished: success.");
			return appRemotePackageInfo;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePack1Info has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePack1Info failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePack1Info finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePack1InfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePack1Info(context.Resolve<GetFilteredAppRemotePack1FilesUnpackInfoDelegate>(), context.Resolve<GetFilteredNewAppRemotePackageSourcesInfoDelegate>(), context.Resolve<GetFilteredAppRemotePackageChunksRangesDelegate>(), context.Resolve<GetFilteredAppRemotePackageBytesRangesDelegate>(), context.Resolve<GetFilteredNewAppRemotePack1FilesUnpackInfoDelegate>(), context.Resolve<GetFilteredNewAppRemotePackageChunksInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
