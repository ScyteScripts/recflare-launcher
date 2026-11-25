using System;
using System.Collections.Generic;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePackageChunksInfo
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageChunksInfo? Execute(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, AppRemotePackageChunksInfo? info)
	{
		if (!info.HasValue)
		{
			return null;
		}
		List<Hash> list = new List<Hash>();
		foreach (FilteredAppRemotePackageChunksRange item in filteredChunksRanges)
		{
			for (long num = item.StartChunkIndex; num <= item.EndChunkIndex; num++)
			{
				int index = (int)num;
				list.Add(info.Value.Hashes[index]);
			}
		}
		ImmutableArray<Hash> hashes = list.AsImmutable();
		return new AppRemotePackageChunksInfo(info.Value.Size, hashes);
	}

	public GetFilteredNewAppRemotePackageChunksInfo(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageChunksInfo? DelegateExecute(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, AppRemotePackageChunksInfo? info)
	{
		Arg.IsValid(filteredChunksRanges, "filteredChunksRanges");
		Arg.IsValid(info, "info");
		LogInfo("Executing GetFilteredNewAppRemotePackageChunksInfo...");
		ImmutableArray<FilteredAppRemotePackageChunksRange> immutableArray = filteredChunksRanges;
		LogTrace("filteredChunksRanges = " + immutableArray.ToString());
		AppRemotePackageChunksInfo? appRemotePackageChunksInfo = info;
		LogTrace("info = " + appRemotePackageChunksInfo.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePackageChunksInfo? appRemotePackageChunksInfo2 = Execute(filteredChunksRanges, info);
			EndLogRegion(enabled: true);
			appRemotePackageChunksInfo = appRemotePackageChunksInfo2;
			LogTrace("result = " + appRemotePackageChunksInfo.ToString());
			LogInfo("GetFilteredNewAppRemotePackageChunksInfo finished: success.");
			return appRemotePackageChunksInfo2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePackageChunksInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageChunksInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageChunksInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePackageChunksInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePackageChunksInfo(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
