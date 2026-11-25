using System;
using System.Linq;
using Autofac;
using PatchKit.Api.Models;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public class GetAppRemotePackageSourcesInfo
{
	public GetAppRemotePackageSourceInfoDelegate GetAppRemotePackageSourceInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppRemotePackageSourceInfo> Execute(ImmutableArray<ResourceUrl> resourceUrls, BytesCount size, Text? authHeaderValue)
	{
		return resourceUrls.Select((ResourceUrl x) =>
		{
			Assert.IsNotNull(x.Url);
			return GetAppRemotePackageSourceInfo(new HttpAddress(new Uri(x.Url)), (x.PartSize > 0) ? new BytesCount?(new BytesCount(x.PartSize)) : ((BytesCount?)null), new BytesCount(0L), size, authHeaderValue);
		}).AsImmutable();
	}

	public GetAppRemotePackageSourcesInfo(GetAppRemotePackageSourceInfoDelegate getAppRemotePackageSourceInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemotePackageSourceInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemotePackageSourceInfo = getAppRemotePackageSourceInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppRemotePackageSourceInfo> DelegateExecute(ImmutableArray<ResourceUrl> resourceUrls, BytesCount size, Text? authHeaderValue)
	{
		Arg.IsValid(size, "size");
		Arg.IsValid(authHeaderValue, "authHeaderValue");
		LogInfo("Executing GetAppRemotePackageSourcesInfo...");
		ImmutableArray<ResourceUrl> immutableArray = resourceUrls;
		LogTrace("resourceUrls = " + immutableArray.ToString());
		BytesCount bytesCount = size;
		LogTrace("size = " + bytesCount.ToString());
		Text? text = authHeaderValue;
		LogTrace("authHeaderValue = " + (text.HasValue ? ((string)text.GetValueOrDefault()) : null));
		try
		{
			BeginLogRegion(enabled: false);
			ImmutableArray<AppRemotePackageSourceInfo> immutableArray2 = Execute(resourceUrls, size, authHeaderValue);
			EndLogRegion(enabled: false);
			ImmutableArray<AppRemotePackageSourceInfo> immutableArray3 = immutableArray2;
			LogTrace("result = " + immutableArray3.ToString());
			LogInfo("GetAppRemotePackageSourcesInfo finished: success.");
			return immutableArray2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePackageSourcesInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePackageSourcesInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePackageSourcesInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemotePackageSourcesInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemotePackageSourcesInfo(context.Resolve<GetAppRemotePackageSourceInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
