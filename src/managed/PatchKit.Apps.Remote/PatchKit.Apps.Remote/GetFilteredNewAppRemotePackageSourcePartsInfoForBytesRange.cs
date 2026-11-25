using System;
using System.Collections.Generic;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange
{
	public GetJointBytesRangeDelegate GetJointBytesRange { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppRemotePackageSourcePartInfo> Execute(ImmutableArray<AppRemotePackageSourcePartInfo> partsInfo, BytesRange filteredBytesRange)
	{
		List<AppRemotePackageSourcePartInfo> list = new List<AppRemotePackageSourcePartInfo>();
		long num = 0L;
		foreach (AppRemotePackageSourcePartInfo item in partsInfo)
		{
			BytesRange other = new BytesRange(num, num + (long)item.Size - 1);
			BytesRange? bytesRange = GetJointBytesRange(filteredBytesRange, other);
			if (bytesRange.HasValue)
			{
				long num2 = bytesRange.Value.Start - other.Start;
				BytesCount offset = new BytesCount((long)item.Offset + num2);
				long v = bytesRange.Value.End - bytesRange.Value.Start + 1;
				list.Add(new AppRemotePackageSourcePartInfo(item.Address, offset, new BytesCount(v), item.AuthHeaderValue));
			}
			num += (long)item.Size;
		}
		return list.AsImmutable();
	}

	public GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange(GetJointBytesRangeDelegate getJointBytesRange, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getJointBytesRange);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetJointBytesRange = getJointBytesRange;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppRemotePackageSourcePartInfo> DelegateExecute(ImmutableArray<AppRemotePackageSourcePartInfo> partsInfo, BytesRange filteredBytesRange)
	{
		Arg.IsValid(partsInfo, "partsInfo");
		Arg.IsValid(filteredBytesRange, "filteredBytesRange");
		LogInfo("Executing GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange...");
		ImmutableArray<AppRemotePackageSourcePartInfo> immutableArray = partsInfo;
		LogTrace("partsInfo = " + immutableArray.ToString());
		BytesRange bytesRange = filteredBytesRange;
		LogTrace("filteredBytesRange = " + bytesRange.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<AppRemotePackageSourcePartInfo> immutableArray2 = Execute(partsInfo, filteredBytesRange);
			EndLogRegion(enabled: true);
			immutableArray = immutableArray2;
			LogTrace("result = " + immutableArray.ToString());
			LogInfo("GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange finished: success.");
			return immutableArray2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRangeDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange(context.Resolve<GetJointBytesRangeDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
