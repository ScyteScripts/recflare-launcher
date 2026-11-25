using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppEntryInfo
{
	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public GetAppMetaDataEntryDelegate GetAppMetaDataEntry { get; }

	public DoesFileExistDelegate DoesFileExist { get; }

	public DoesDirExistDelegate DoesDirExist { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppEntryInfo? Execute(App app, AppPath path)
	{
		Path path2 = GetAppEntryFullPath(app, path);
		AppMetaDataEntry? appMetaDataEntry = GetAppMetaDataEntry(app, path, null);
		if (!appMetaDataEntry.HasValue)
		{
			return null;
		}
		if (DoesDirExist(path2, null))
		{
			return new AppEntryInfo(path, AppEntryType.Directory, appMetaDataEntry.Value.VersionId);
		}
		if (DoesFileExist(path2, null))
		{
			return new AppEntryInfo(path, AppEntryType.File, appMetaDataEntry.Value.VersionId);
		}
		return null;
	}

	public GetAppEntryInfo(GetAppEntryFullPathDelegate getAppEntryFullPath, GetAppMetaDataEntryDelegate getAppMetaDataEntry, DoesFileExistDelegate doesFileExist, DoesDirExistDelegate doesDirExist, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(getAppMetaDataEntry);
		Assert.IsNotNull(doesFileExist);
		Assert.IsNotNull(doesDirExist);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryFullPath = getAppEntryFullPath;
		GetAppMetaDataEntry = getAppMetaDataEntry;
		DoesFileExist = doesFileExist;
		DoesDirExist = doesDirExist;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppEntryInfo? DelegateExecute(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetAppEntryInfo...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			AppEntryInfo? appEntryInfo = Execute(app, path);
			EndLogRegion(enabled: true);
			AppEntryInfo? appEntryInfo2 = appEntryInfo;
			LogTrace("result = " + appEntryInfo2.ToString());
			LogInfo("GetAppEntryInfo finished: success.");
			return appEntryInfo;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppEntryInfo finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppEntryInfo?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppEntryInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppEntryInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppEntryInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppEntryInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppEntryInfo(context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<GetAppMetaDataEntryDelegate>(), context.Resolve<DoesFileExistDelegate>(), context.Resolve<DoesDirExistDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
