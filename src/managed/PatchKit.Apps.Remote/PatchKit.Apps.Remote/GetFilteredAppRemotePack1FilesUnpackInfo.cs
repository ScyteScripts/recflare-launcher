using System;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredAppRemotePack1FilesUnpackInfo
{
	public GetFilteredAppRemotePack1FileUnpackInfoDelegate GetFilteredAppRemotePack1FileUnpackInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> Execute(ImmutableArray<AppRemotePack1FileUnpackInfo> filesUnpackInfo, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo, BytesCount chunkSize)
	{
		return (from fileUnpackInfo in filesUnpackInfo
			where newFilesInfo.Any((AppRemotePackageFileInfo x) => (string)x.Path == (string)fileUnpackInfo.Path)
			select GetFilteredAppRemotePack1FileUnpackInfo(fileUnpackInfo, chunkSize)).AsImmutable();
	}

	public GetFilteredAppRemotePack1FilesUnpackInfo(GetFilteredAppRemotePack1FileUnpackInfoDelegate getFilteredAppRemotePack1FileUnpackInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredAppRemotePack1FileUnpackInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredAppRemotePack1FileUnpackInfo = getFilteredAppRemotePack1FileUnpackInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> DelegateExecute(ImmutableArray<AppRemotePack1FileUnpackInfo> filesUnpackInfo, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo, BytesCount chunkSize)
	{
		Arg.IsValid(filesUnpackInfo, "filesUnpackInfo");
		Arg.IsValid(newFilesInfo, "newFilesInfo");
		Arg.IsValid(chunkSize, "chunkSize");
		LogInfo("Executing GetFilteredAppRemotePack1FilesUnpackInfo...");
		ImmutableArray<AppRemotePack1FileUnpackInfo> immutableArray = filesUnpackInfo;
		LogTrace("filesUnpackInfo = " + immutableArray.ToString());
		ImmutableArray<AppRemotePackageFileInfo> immutableArray2 = newFilesInfo;
		LogTrace("newFilesInfo = " + immutableArray2.ToString());
		BytesCount bytesCount = chunkSize;
		LogTrace("chunkSize = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> immutableArray3 = Execute(filesUnpackInfo, newFilesInfo, chunkSize);
			EndLogRegion(enabled: true);
			ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> immutableArray4 = immutableArray3;
			LogTrace("result = " + immutableArray4.ToString());
			LogInfo("GetFilteredAppRemotePack1FilesUnpackInfo finished: success.");
			return immutableArray3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredAppRemotePack1FilesUnpackInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePack1FilesUnpackInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePack1FilesUnpackInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredAppRemotePack1FilesUnpackInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredAppRemotePack1FilesUnpackInfo(context.Resolve<GetFilteredAppRemotePack1FileUnpackInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
