using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class SetAppMetaDataEntry
{
	public ReadAppMetaDataDelegate ReadAppMetaData { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path, AppVersionId versionId)
	{
		AppMetaData appMetaData = ReadAppMetaData(app, null);
		List<AppMetaDataEntry> list = appMetaData.Entries.ToList();
		list.RemoveAll((AppMetaDataEntry x) => (string)x.Path == (string)path);
		list.Add(new AppMetaDataEntry(path, versionId));
		app.MetaDataCache = appMetaData.ModifyEntries(list.AsImmutable());
	}

	public SetAppMetaDataEntry(ReadAppMetaDataDelegate readAppMetaData, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

	private object DelegateExecute(App app, AppPath path, AppVersionId versionId, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		Arg.IsValid(versionId, "versionId");
		LogInfo("Executing SetAppMetaDataEntry...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path, versionId);
			EndLogRegion(enabled: true);
			LogInfo("SetAppMetaDataEntry finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppMetaDataEntry finished: UnauthorizedAccess.");
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
			LogInfo("SetAppMetaDataEntry has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppMetaDataEntry failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppMetaDataEntry finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SetAppMetaDataEntryDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SetAppMetaDataEntry(context.Resolve<ReadAppMetaDataDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
