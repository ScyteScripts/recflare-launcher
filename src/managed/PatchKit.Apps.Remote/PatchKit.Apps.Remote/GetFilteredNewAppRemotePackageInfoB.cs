using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class GetFilteredNewAppRemotePackageInfoB
{
	public GetFilteredNewAppRemotePackageInfoADelegate GetFilteredNewAppRemotePackageInfoA { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageInfo Execute(AppRemotePackageInfo info, ImmutableArray<AppPath> dataFiles, ImmutableArray<AppPath> patchFiles)
	{
		HashSet<AppPath> dataFilesSet = new HashSet<AppPath>(dataFiles);
		HashSet<AppPath> patchFilesSet = new HashSet<AppPath>(patchFiles);
		ImmutableArray<AppRemotePackageFileInfo> newFilesInfo = info.FilesInfo.Where((AppRemotePackageFileInfo x) =>
		{
			if (x.DataFileInfo.HasValue && Enumerable.Contains(dataFilesSet, x.Path))
			{
				return true;
			}
			return (x.PatchFileInfo.HasValue && Enumerable.Contains(patchFilesSet, x.Path)) ? true : false;
		}).AsImmutable();
		return GetFilteredNewAppRemotePackageInfoA(info, newFilesInfo);
	}

	public GetFilteredNewAppRemotePackageInfoB(GetFilteredNewAppRemotePackageInfoADelegate getFilteredNewAppRemotePackageInfoA, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFilteredNewAppRemotePackageInfoA);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFilteredNewAppRemotePackageInfoA = getFilteredNewAppRemotePackageInfoA;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageInfo DelegateExecute(AppRemotePackageInfo info, ImmutableArray<AppPath> dataFiles, ImmutableArray<AppPath> patchFiles)
	{
		Arg.IsValid(info, "info");
		Arg.IsValid(dataFiles, "dataFiles");
		Arg.IsValid(patchFiles, "patchFiles");
		LogInfo("Executing GetFilteredNewAppRemotePackageInfoB...");
		AppRemotePackageInfo appRemotePackageInfo = info;
		LogTrace("info = " + appRemotePackageInfo.ToString());
		ImmutableArray<AppPath> immutableArray = dataFiles;
		LogTrace("dataFiles = " + immutableArray.ToString());
		immutableArray = patchFiles;
		LogTrace("patchFiles = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			AppRemotePackageInfo appRemotePackageInfo2 = Execute(info, dataFiles, patchFiles);
			EndLogRegion(enabled: true);
			appRemotePackageInfo = appRemotePackageInfo2;
			LogTrace("result = " + appRemotePackageInfo.ToString());
			LogInfo("GetFilteredNewAppRemotePackageInfoB finished: success.");
			return appRemotePackageInfo2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilteredNewAppRemotePackageInfoB has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageInfoB failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilteredNewAppRemotePackageInfoB finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilteredNewAppRemotePackageInfoBDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilteredNewAppRemotePackageInfoB(context.Resolve<GetFilteredNewAppRemotePackageInfoADelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
