using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePackageInfoA
{
	public GetFilteredNewAppRemotePack1InfoDelegate GetFilteredNewAppRemotePack1Info { get; }

	public GetFilteredNewAppRemoteZipInfoDelegate GetFilteredNewAppRemoteZipInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageInfo Execute(AppRemotePackageInfo info, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo)
	{
		if (info.UnpackInfo.ZipInfo.HasValue)
		{
			return GetFilteredNewAppRemoteZipInfo(info.UnpackInfo.ZipInfo.Value, info.SourcesInfo, info.ChunksInfo, info.Size, newFilesInfo);
		}
		if (info.UnpackInfo.Pack1Info.HasValue)
		{
			return GetFilteredNewAppRemotePack1Info(info.UnpackInfo.Pack1Info.Value, info.SourcesInfo, info.ChunksInfo, info.Size, newFilesInfo);
		}
		throw new UnpredictedCondition("Unsupported unpack method");
	}

	public GetFilteredNewAppRemotePackageInfoA(GetFilteredNewAppRemotePack1InfoDelegate getFilteredNewAppRemotePack1Info, GetFilteredNewAppRemoteZipInfoDelegate getFilteredNewAppRemoteZipInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredNewAppRemotePack1Info);
		Assert.IsNotNull(getFilteredNewAppRemoteZipInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredNewAppRemotePack1Info = getFilteredNewAppRemotePack1Info;
		GetFilteredNewAppRemoteZipInfo = getFilteredNewAppRemoteZipInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageInfo DelegateExecute(AppRemotePackageInfo info, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo)
	{
		Arg.IsValid(info, "info");
		Arg.IsValid(newFilesInfo, "newFilesInfo");
		LogInfo("Executing GetFilteredNewAppRemotePackageInfoA...");
		AppRemotePackageInfo appRemotePackageInfo = info;
		LogTrace("info = " + appRemotePackageInfo.ToString());
		ImmutableArray<AppRemotePackageFileInfo> immutableArray = newFilesInfo;
		LogTrace("newFilesInfo = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo2 = Execute(info, newFilesInfo);
			EndLogRegion(enabled: true);
			appRemotePackageInfo = appRemotePackageInfo2;
			LogTrace("result = " + appRemotePackageInfo.ToString());
			LogInfo("GetFilteredNewAppRemotePackageInfoA finished: success.");
			return appRemotePackageInfo2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePackageInfoA has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageInfoA failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageInfoA finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePackageInfoADelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePackageInfoA(context.Resolve<GetFilteredNewAppRemotePack1InfoDelegate>(), context.Resolve<GetFilteredNewAppRemoteZipInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
