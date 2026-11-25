using System;
using System.Collections.Generic;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePackageSourceInfo
{
	public GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRangeDelegate GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageSourceInfo Execute(AppRemotePackageSourceInfo sourceInfo, ImmutableArray<BytesRange> filteredBytesRanges)
	{
		List<AppRemotePackageSourcePartInfo> list = new List<AppRemotePackageSourcePartInfo>();
		foreach (BytesRange item in filteredBytesRanges)
		{
			ImmutableArray<AppRemotePackageSourcePartInfo> immutableArray = GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange(sourceInfo.PartsInfo, item);
			list.AddRange(immutableArray);
		}
		return new AppRemotePackageSourceInfo(list.AsImmutable());
	}

	public GetFilteredNewAppRemotePackageSourceInfo(GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRangeDelegate getFilteredNewAppRemotePackageSourcePartsInfoForBytesRange, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredNewAppRemotePackageSourcePartsInfoForBytesRange);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange = getFilteredNewAppRemotePackageSourcePartsInfoForBytesRange;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageSourceInfo DelegateExecute(AppRemotePackageSourceInfo sourceInfo, ImmutableArray<BytesRange> filteredBytesRanges)
	{
		Arg.IsValid(sourceInfo, "sourceInfo");
		Arg.IsValid(filteredBytesRanges, "filteredBytesRanges");
		LogInfo("Executing GetFilteredNewAppRemotePackageSourceInfo...");
		AppRemotePackageSourceInfo appRemotePackageSourceInfo = sourceInfo;
		LogTrace("sourceInfo = " + appRemotePackageSourceInfo.ToString());
		ImmutableArray<BytesRange> immutableArray = filteredBytesRanges;
		LogTrace("filteredBytesRanges = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePackageSourceInfo appRemotePackageSourceInfo2 = Execute(sourceInfo, filteredBytesRanges);
			EndLogRegion(enabled: true);
			appRemotePackageSourceInfo = appRemotePackageSourceInfo2;
			LogTrace("result = " + appRemotePackageSourceInfo.ToString());
			LogInfo("GetFilteredNewAppRemotePackageSourceInfo finished: success.");
			return appRemotePackageSourceInfo2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePackageSourceInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageSourceInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageSourceInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePackageSourceInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePackageSourceInfo(context.Resolve<GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRangeDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
