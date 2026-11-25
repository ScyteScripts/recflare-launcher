using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class SetAppInfo
{
	public ReadAppMetaDataDelegate ReadAppMetaData { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppInfo value)
	{
		AppMetaData value2 = ReadAppMetaData(app, null).ModifyInfo(value);
		app.MetaDataCache = value2;
	}

	public SetAppInfo(ReadAppMetaDataDelegate readAppMetaData, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(readAppMetaData);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ReadAppMetaData = readAppMetaData;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppInfo value, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(value, "value");
		LogInfo("Executing SetAppInfo...");
		LogTrace("app = " + app);
		AppInfo appInfo = value;
		LogTrace("value = " + appInfo.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, value);
			EndLogRegion(enabled: true);
			LogInfo("SetAppInfo finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppInfo finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SetAppInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SetAppInfo(context.Resolve<ReadAppMetaDataDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
