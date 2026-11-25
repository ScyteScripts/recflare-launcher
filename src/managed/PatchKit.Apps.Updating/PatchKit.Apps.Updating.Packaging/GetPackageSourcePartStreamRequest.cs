using System;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackageSourcePartStreamRequest
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private PackageSourcePartStreamRequest? Execute(PackageSourceStreamContext context)
	{
		long num = 0L;
		foreach (AppRemotePackageSourcePartInfo item in context.Info.PartsInfo)
		{
			long num2 = num + (long)item.Size;
			if ((long)context.Position < num2)
			{
				return new PackageSourcePartStreamRequest(item, new BytesCount((long)context.Position - num));
			}
			num = num2;
		}
		return null;
	}

	public GetPackageSourcePartStreamRequest(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private PackageSourcePartStreamRequest? DelegateExecute(PackageSourceStreamContext context)
	{
		Arg.IsValid(context, "context");
		LogInfo("Executing GetPackageSourcePartStreamRequest...");
		PackageSourceStreamContext packageSourceStreamContext = context;
		LogTrace("context = " + packageSourceStreamContext.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			PackageSourcePartStreamRequest? packageSourcePartStreamRequest = Execute(context);
			EndLogRegion(enabled: true);
			PackageSourcePartStreamRequest? packageSourcePartStreamRequest2 = packageSourcePartStreamRequest;
			LogTrace("result = " + packageSourcePartStreamRequest2.ToString());
			LogInfo("GetPackageSourcePartStreamRequest finished: success.");
			return packageSourcePartStreamRequest;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackageSourcePartStreamRequest has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageSourcePartStreamRequest failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageSourcePartStreamRequest finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageSourcePartStreamRequestDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackageSourcePartStreamRequest(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
