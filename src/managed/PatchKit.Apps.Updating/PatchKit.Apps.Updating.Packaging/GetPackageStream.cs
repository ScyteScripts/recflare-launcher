using System;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackageStream
{
	public GetPackageWithChunksStreamDelegate GetPackageWithChunksStream { get; }

	public GetPackageWithoutChunksStreamDelegate GetPackageWithoutChunksStream { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageStreamDisposable Execute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size)
	{
		if (chunksInfo.HasValue)
		{
			return GetPackageWithChunksStream(sourcesInfo, chunksInfo.Value, size);
		}
		return GetPackageWithoutChunksStream(sourcesInfo, size);
	}

	public GetPackageStream(GetPackageWithChunksStreamDelegate getPackageWithChunksStream, GetPackageWithoutChunksStreamDelegate getPackageWithoutChunksStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getPackageWithChunksStream);
		Assert.IsNotNull(getPackageWithoutChunksStream);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetPackageWithChunksStream = getPackageWithChunksStream;
		GetPackageWithoutChunksStream = getPackageWithoutChunksStream;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageStreamDisposable DelegateExecute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size)
	{
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(chunksInfo, "chunksInfo");
		Arg.IsValid(size, "size");
		LogInfo("Executing GetPackageStream...");
		ImmutableArray<AppRemotePackageSourceInfo> immutableArray = sourcesInfo;
		LogTrace("sourcesInfo = " + immutableArray.ToString());
		AppRemotePackageChunksInfo? appRemotePackageChunksInfo = chunksInfo;
		LogTrace("chunksInfo = " + appRemotePackageChunksInfo.ToString());
		BytesCount bytesCount = size;
		LogTrace("size = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageStreamDisposable packageStreamDisposable = Execute(sourcesInfo, chunksInfo, size);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageStreamDisposable);
			LogInfo("GetPackageStream finished: success.");
			return packageStreamDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackageStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackageStream(context.Resolve<GetPackageWithChunksStreamDelegate>(), context.Resolve<GetPackageWithoutChunksStreamDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
