using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppMetaDataEntry
{
	public ReadAppMetaDataDelegate ReadAppMetaData { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaDataEntry? Execute(App app, AppPath path)
	{
		return ReadAppMetaData(app, null).Entries.FirstOrNull((AppMetaDataEntry x) => (string)x.Path == (string)path);
	}

	public GetAppMetaDataEntry(ReadAppMetaDataDelegate readAppMetaData, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

	private AppMetaDataEntry? DelegateExecute(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetAppMetaDataEntry...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			AppMetaDataEntry? appMetaDataEntry = Execute(app, path);
			EndLogRegion(enabled: true);
			AppMetaDataEntry? appMetaDataEntry2 = appMetaDataEntry;
			LogTrace("result = " + appMetaDataEntry2.ToString());
			LogInfo("GetAppMetaDataEntry finished: success.");
			return appMetaDataEntry;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppMetaDataEntry finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppMetaDataEntry?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppMetaDataEntry has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppMetaDataEntry failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppMetaDataEntry finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppMetaDataEntryDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppMetaDataEntry(context.Resolve<ReadAppMetaDataDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
