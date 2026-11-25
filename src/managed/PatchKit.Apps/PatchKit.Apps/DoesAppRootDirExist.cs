using System;
using Autofac;
using PatchKit.Apps.Serialization;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class DoesAppRootDirExist
{
	public ReadAppMetaDataFromFileDelegate ReadAppMetaDataFromFile { get; }

	public GetAppMetaDataFilePathDelegate GetAppMetaDataFilePath { get; }

	public GetAppMetaDataDeprecatedFilePathsDelegate GetAppMetaDataDeprecatedFilePaths { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(App app)
	{
		if (ReadAppMetaDataFromFile(GetAppMetaDataFilePath(app), (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}, (NotExistingFile e) => (object)null, (MetaDataDeserializeFailure e) => (object)null, (UnsupportedMetaDataFormatVersion e) => (object)null).HasValue)
		{
			return true;
		}
		foreach (Path item in GetAppMetaDataDeprecatedFilePaths(app))
		{
			if (ReadAppMetaDataFromFile(item, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppRootDir();
			}, (NotExistingFile e) => (object)null, (MetaDataDeserializeFailure e) => (object)null, (UnsupportedMetaDataFormatVersion e) => (object)null).HasValue)
			{
				return true;
			}
		}
		return false;
	}

	public DoesAppRootDirExist(ReadAppMetaDataFromFileDelegate readAppMetaDataFromFile, GetAppMetaDataFilePathDelegate getAppMetaDataFilePath, GetAppMetaDataDeprecatedFilePathsDelegate getAppMetaDataDeprecatedFilePaths, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(readAppMetaDataFromFile);
		Assert.IsNotNull(getAppMetaDataFilePath);
		Assert.IsNotNull(getAppMetaDataDeprecatedFilePaths);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ReadAppMetaDataFromFile = readAppMetaDataFromFile;
		GetAppMetaDataFilePath = getAppMetaDataFilePath;
		GetAppMetaDataDeprecatedFilePaths = getAppMetaDataDeprecatedFilePaths;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing DoesAppRootDirExist...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: false);
			bool result = Execute(app);
			EndLogRegion(enabled: false);
			LogTrace("result = " + result);
			LogInfo("DoesAppRootDirExist finished: success.");
			return result;
		}
		catch (UnauthorizedAccessToAppRootDir e)
		{
			EndLogRegion(enabled: false);
			LogInfo("DoesAppRootDirExist finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccessToAppRootDir(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("DoesAppRootDirExist has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("DoesAppRootDirExist failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("DoesAppRootDirExist finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DoesAppRootDirExistDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DoesAppRootDirExist(context.Resolve<ReadAppMetaDataFromFileDelegate>(), context.Resolve<GetAppMetaDataFilePathDelegate>(), context.Resolve<GetAppMetaDataDeprecatedFilePathsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
