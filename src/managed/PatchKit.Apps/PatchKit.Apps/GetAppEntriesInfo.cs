using System;
using System.Collections.Generic;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppEntriesInfo
{
	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public ReadAppMetaDataDelegate ReadAppMetaData { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppEntryInfo> Execute(App app)
	{
		return Enumerate(app).AsImmutable();
	}

	private IEnumerable<AppEntryInfo> Enumerate(App app)
	{
		foreach (AppMetaDataEntry entry in ReadAppMetaData(app, null).Entries)
		{
			AppEntryInfo? entryInfo = GetAppEntryInfo(app, entry.Path, null);
			if (entryInfo.HasValue)
			{
				yield return entryInfo.Value;
			}
		}
	}

	public GetAppEntriesInfo(GetAppEntryInfoDelegate getAppEntryInfo, ReadAppMetaDataDelegate readAppMetaData, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(readAppMetaData);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryInfo = getAppEntryInfo;
		ReadAppMetaData = readAppMetaData;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppEntryInfo> DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GetAppEntriesInfo...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: false);
			ImmutableArray<AppEntryInfo> immutableArray = Execute(app);
			EndLogRegion(enabled: false);
			ImmutableArray<AppEntryInfo> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("GetAppEntriesInfo finished: success.");
			return immutableArray;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppEntriesInfo finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (ImmutableArray<AppEntryInfo>)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppEntriesInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppEntriesInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppEntriesInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppEntriesInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppEntriesInfo(context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<ReadAppMetaDataDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
