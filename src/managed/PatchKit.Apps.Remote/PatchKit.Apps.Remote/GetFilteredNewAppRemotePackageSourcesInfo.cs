using System;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePackageSourcesInfo
{
	public GetFilteredNewAppRemotePackageSourceInfoDelegate GetFilteredNewAppRemotePackageSourceInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppRemotePackageSourceInfo> Execute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, ImmutableArray<BytesRange> filteredBytesRanges)
	{
		return sourcesInfo.Select((AppRemotePackageSourceInfo sourceInfo) => GetFilteredNewAppRemotePackageSourceInfo(sourceInfo, filteredBytesRanges)).AsImmutable();
	}

	public GetFilteredNewAppRemotePackageSourcesInfo(GetFilteredNewAppRemotePackageSourceInfoDelegate getFilteredNewAppRemotePackageSourceInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredNewAppRemotePackageSourceInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredNewAppRemotePackageSourceInfo = getFilteredNewAppRemotePackageSourceInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppRemotePackageSourceInfo> DelegateExecute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, ImmutableArray<BytesRange> filteredBytesRanges)
	{
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(filteredBytesRanges, "filteredBytesRanges");
		LogInfo("Executing GetFilteredNewAppRemotePackageSourcesInfo...");
		ImmutableArray<AppRemotePackageSourceInfo> immutableArray = sourcesInfo;
		LogTrace("sourcesInfo = " + immutableArray.ToString());
		ImmutableArray<BytesRange> immutableArray2 = filteredBytesRanges;
		LogTrace("filteredBytesRanges = " + immutableArray2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<AppRemotePackageSourceInfo> immutableArray3 = Execute(sourcesInfo, filteredBytesRanges);
			EndLogRegion(enabled: true);
			immutableArray = immutableArray3;
			LogTrace("result = " + immutableArray.ToString());
			LogInfo("GetFilteredNewAppRemotePackageSourcesInfo finished: success.");
			return immutableArray3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePackageSourcesInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageSourcesInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageSourcesInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePackageSourcesInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePackageSourcesInfo(context.Resolve<GetFilteredNewAppRemotePackageSourceInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
