using System;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackage
{
	public GetZipDelegate GetZip { get; }

	public GetPack1Delegate GetPack1 { get; }

	public GetPackageStreamDelegate GetPackageStream { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageDisposable Execute(AppRemotePackageInfo info)
	{
		IPackageStreamDisposable packageStreamDisposable = GetPackageStream(info.SourcesInfo, size: info.Size, chunksInfo: info.ChunksInfo);
		Assert.IsNotNullFromDelegate(packageStreamDisposable);
		IPackageDisposable packageDisposable = null;
		if (info.UnpackInfo.ZipInfo.HasValue)
		{
			AppRemoteZipUnpackInfo value = info.UnpackInfo.ZipInfo.Value;
			packageDisposable = GetZip(info.FilesInfo, value, packageStreamDisposable, info.Size);
		}
		else if (info.UnpackInfo.Pack1Info.HasValue)
		{
			AppRemotePack1UnpackInfo value2 = info.UnpackInfo.Pack1Info.Value;
			packageDisposable = GetPack1(info.FilesInfo, value2, packageStreamDisposable, info.Size);
		}
		Assert.IsNotNull(packageDisposable);
		packageDisposable.AttachDisposable(packageStreamDisposable);
		return packageDisposable;
	}

	public GetPackage(GetZipDelegate getZip, GetPack1Delegate getPack1, GetPackageStreamDelegate getPackageStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getZip);
		Assert.IsNotNull(getPack1);
		Assert.IsNotNull(getPackageStream);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetZip = getZip;
		GetPack1 = getPack1;
		GetPackageStream = getPackageStream;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageDisposable DelegateExecute(AppRemotePackageInfo info)
	{
		Arg.IsValid(info, "info");
		LogInfo("Executing GetPackage...");
		AppRemotePackageInfo appRemotePackageInfo = info;
		LogTrace("info = " + appRemotePackageInfo.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageDisposable packageDisposable = Execute(info);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageDisposable);
			LogInfo("GetPackage finished: success.");
			return packageDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackage has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackage failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackage finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackage(context.Resolve<GetZipDelegate>(), context.Resolve<GetPack1Delegate>(), context.Resolve<GetPackageStreamDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
