using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemoteZipInfo
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageInfo Execute(AppRemoteZipUnpackInfo unpackInfo, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo)
	{
		return new AppRemotePackageInfo(newFilesInfo, unpackInfo, sourcesInfo, chunksInfo, size);
	}

	public GetFilteredNewAppRemoteZipInfo(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageInfo DelegateExecute(AppRemoteZipUnpackInfo unpackInfo, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo)
	{
		Arg.IsValid(unpackInfo, "unpackInfo");
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(chunksInfo, "chunksInfo");
		Arg.IsValid(size, "size");
		Arg.IsValid(newFilesInfo, "newFilesInfo");
		LogInfo("Executing GetFilteredNewAppRemoteZipInfo...");
		AppRemoteZipUnpackInfo appRemoteZipUnpackInfo = unpackInfo;
		LogTrace("unpackInfo = " + appRemoteZipUnpackInfo.ToString());
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
			LogInfo("GetFilteredNewAppRemoteZipInfo finished: success.");
			return appRemotePackageInfo;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemoteZipInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemoteZipInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemoteZipInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemoteZipInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemoteZipInfo(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
