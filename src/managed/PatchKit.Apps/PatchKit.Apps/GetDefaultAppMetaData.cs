using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class GetDefaultAppMetaData
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaData Execute()
	{
		return new AppMetaData(new AppInfo(null, null, null, null, null, null, null, null, null), ImmutableArray<AppMetaDataEntry>.Empty);
	}

	public GetDefaultAppMetaData(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppMetaData DelegateExecute()
	{
		LogInfo("Executing GetDefaultAppMetaData...");
		try
		{
			BeginLogRegion(enabled: false);
			AppMetaData appMetaData = Execute();
			EndLogRegion(enabled: false);
			AppMetaData appMetaData2 = appMetaData;
			LogTrace("result = " + appMetaData2.ToString());
			LogInfo("GetDefaultAppMetaData finished: success.");
			return appMetaData;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetDefaultAppMetaData has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetDefaultAppMetaData failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetDefaultAppMetaData finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetDefaultAppMetaDataDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetDefaultAppMetaData(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
