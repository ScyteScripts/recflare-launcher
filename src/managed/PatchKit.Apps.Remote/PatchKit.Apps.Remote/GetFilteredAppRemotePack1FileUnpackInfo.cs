using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredAppRemotePack1FileUnpackInfo
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private FilteredAppRemotePack1FileUnpackInfo Execute(AppRemotePack1FileUnpackInfo fileUnpackInfo, BytesCount chunkSize)
	{
		long startChunkIndex = (long)fileUnpackInfo.Offset / (long)chunkSize;
		long v = (long)fileUnpackInfo.Offset % (long)chunkSize;
		long endChunkIndex = ((long)(fileUnpackInfo.Offset + fileUnpackInfo.Size) - 1) / (long)chunkSize;
		return new FilteredAppRemotePack1FileUnpackInfo(fileUnpackInfo.Path, fileUnpackInfo.Size, new BytesCount(v), new FilteredAppRemotePackageChunksRange(startChunkIndex, endChunkIndex));
	}

	public GetFilteredAppRemotePack1FileUnpackInfo(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private FilteredAppRemotePack1FileUnpackInfo DelegateExecute(AppRemotePack1FileUnpackInfo fileUnpackInfo, BytesCount chunkSize)
	{
		Arg.IsValid(fileUnpackInfo, "fileUnpackInfo");
		Arg.IsValid(chunkSize, "chunkSize");
		LogInfo("Executing GetFilteredAppRemotePack1FileUnpackInfo...");
		AppRemotePack1FileUnpackInfo appRemotePack1FileUnpackInfo = fileUnpackInfo;
		LogTrace("fileUnpackInfo = " + appRemotePack1FileUnpackInfo.ToString());
		BytesCount bytesCount = chunkSize;
		LogTrace("chunkSize = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			FilteredAppRemotePack1FileUnpackInfo filteredAppRemotePack1FileUnpackInfo = Execute(fileUnpackInfo, chunkSize);
			EndLogRegion(enabled: true);
			FilteredAppRemotePack1FileUnpackInfo filteredAppRemotePack1FileUnpackInfo2 = filteredAppRemotePack1FileUnpackInfo;
			LogTrace("result = " + filteredAppRemotePack1FileUnpackInfo2.ToString());
			LogInfo("GetFilteredAppRemotePack1FileUnpackInfo finished: success.");
			return filteredAppRemotePack1FileUnpackInfo;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredAppRemotePack1FileUnpackInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePack1FileUnpackInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredAppRemotePack1FileUnpackInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredAppRemotePack1FileUnpackInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredAppRemotePack1FileUnpackInfo(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
