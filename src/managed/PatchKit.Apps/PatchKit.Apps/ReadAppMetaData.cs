using System;
using Autofac;
using PatchKit.Apps.Serialization;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class ReadAppMetaData
{
	public ReadAppMetaDataFromFileDelegate ReadAppMetaDataFromFile { get; }

	public GetAppMetaDataFilePathDelegate GetAppMetaDataFilePath { get; }

	public GetAppMetaDataDeprecatedFilePathsDelegate GetAppMetaDataDeprecatedFilePaths { get; }

	public GetDefaultAppMetaDataDelegate GetDefaultAppMetaData { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaData Execute(App app)
	{
		if (app.MetaDataCache.HasValue)
		{
			return app.MetaDataCache.Value;
		}
		AppMetaData defaultResult = GetDefaultAppMetaData();
		AppMetaData? appMetaData = ReadAppMetaDataFromFile(GetAppMetaDataFilePath(app), null, (NotExistingFile e) => (object)null, (MetaDataDeserializeFailure e) => defaultResult, (UnsupportedMetaDataFormatVersion e) => defaultResult);
		if (!appMetaData.HasValue)
		{
			foreach (Path item in GetAppMetaDataDeprecatedFilePaths(app))
			{
				appMetaData = ReadAppMetaDataFromFile(item, null, (NotExistingFile e) => (object)null, (MetaDataDeserializeFailure e) => (object)null, (UnsupportedMetaDataFormatVersion e) => (object)null);
				if (appMetaData.HasValue)
				{
					break;
				}
			}
		}
		appMetaData = appMetaData ?? defaultResult;
		app.MetaDataCache = appMetaData.Value;
		return appMetaData.Value;
	}

	public ReadAppMetaData(ReadAppMetaDataFromFileDelegate readAppMetaDataFromFile, GetAppMetaDataFilePathDelegate getAppMetaDataFilePath, GetAppMetaDataDeprecatedFilePathsDelegate getAppMetaDataDeprecatedFilePaths, GetDefaultAppMetaDataDelegate getDefaultAppMetaData, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(readAppMetaDataFromFile);
		Assert.IsNotNull(getAppMetaDataFilePath);
		Assert.IsNotNull(getAppMetaDataDeprecatedFilePaths);
		Assert.IsNotNull(getDefaultAppMetaData);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ReadAppMetaDataFromFile = readAppMetaDataFromFile;
		GetAppMetaDataFilePath = getAppMetaDataFilePath;
		GetAppMetaDataDeprecatedFilePaths = getAppMetaDataDeprecatedFilePaths;
		GetDefaultAppMetaData = getDefaultAppMetaData;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppMetaData DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing ReadAppMetaData...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: false);
			AppMetaData appMetaData = Execute(app);
			EndLogRegion(enabled: false);
			AppMetaData appMetaData2 = appMetaData;
			LogTrace("result = " + appMetaData2.ToString());
			LogInfo("ReadAppMetaData finished: success.");
			return appMetaData;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppMetaData finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (AppMetaData)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppMetaData has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("ReadAppMetaData failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("ReadAppMetaData finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ReadAppMetaDataDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ReadAppMetaData(context.Resolve<ReadAppMetaDataFromFileDelegate>(), context.Resolve<GetAppMetaDataFilePathDelegate>(), context.Resolve<GetAppMetaDataDeprecatedFilePathsDelegate>(), context.Resolve<GetDefaultAppMetaDataDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
