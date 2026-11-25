using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class DeleteAppFile
{
	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public DeleteFileDelegate DeleteFile { get; }

	public DeleteAppMetaDataEntryDelegate DeleteAppMetaDataEntry { get; }

	public CleanUnregisteredAppEmptyParentDirsDelegate CleanUnregisteredAppEmptyParentDirs { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path)
	{
		AppEntryInfo? appEntryInfo = GetAppEntryInfo(app, path, null);
		if (appEntryInfo.HasValue && appEntryInfo.Value.Type == AppEntryType.File)
		{
			Path path2 = GetAppEntryFullPath(app, path);
			DeleteFile(path2, null, (NotExistingFile e) =>
			{
				throw new UnpredictedException(e);
			});
			DeleteAppMetaDataEntry(app, path, null);
			CleanUnregisteredAppEmptyParentDirs(app, path, null);
			return;
		}
		throw new NotExistingAppFile(app, path);
	}

	public DeleteAppFile(GetAppEntryInfoDelegate getAppEntryInfo, GetAppEntryFullPathDelegate getAppEntryFullPath, DeleteFileDelegate deleteFile, DeleteAppMetaDataEntryDelegate deleteAppMetaDataEntry, CleanUnregisteredAppEmptyParentDirsDelegate cleanUnregisteredAppEmptyParentDirs, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(deleteFile);
		Assert.IsNotNull(deleteAppMetaDataEntry);
		Assert.IsNotNull(cleanUnregisteredAppEmptyParentDirs);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryInfo = getAppEntryInfo;
		GetAppEntryFullPath = getAppEntryFullPath;
		DeleteFile = deleteFile;
		DeleteAppMetaDataEntry = deleteAppMetaDataEntry;
		CleanUnregisteredAppEmptyParentDirs = cleanUnregisteredAppEmptyParentDirs;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingAppFile> onNotExistingAppFile)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing DeleteAppFile...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path);
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (NotExistingAppFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppFile finished: NotExistingAppFile.");
			if (onNotExistingAppFile == null)
			{
				throw;
			}
			onNotExistingAppFile(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeleteAppFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeleteAppFile(context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<DeleteFileDelegate>(), context.Resolve<DeleteAppMetaDataEntryDelegate>(), context.Resolve<CleanUnregisteredAppEmptyParentDirsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
