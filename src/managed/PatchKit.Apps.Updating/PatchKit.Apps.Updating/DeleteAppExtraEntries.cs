using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class DeleteAppExtraEntries
{
	public ReadAppExtraEntriesInfoDelegate ReadAppExtraEntriesInfo { get; }

	public CombinePathDelegate CombinePath { get; }

	public DeleteDirRecursivelyIfExistsDelegate DeleteDirRecursivelyIfExists { get; }

	public DeleteFileIfExistsDelegate DeleteFileIfExists { get; }

	public GetAppExtraEntriesInfoFilePathDelegate GetAppExtraEntriesInfoFilePath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app)
	{
		foreach (AppExtraEntryInfo item in ReadAppExtraEntriesInfo(app, null))
		{
			Path path = CombinePath(app.Path, item.Path);
			DeleteFileIfExists(path, (UnauthorizedAccess e) =>
			{
			});
			DeleteDirRecursivelyIfExists(path, (UnauthorizedAccess e) =>
			{
			});
		}
		DeleteFileIfExists(GetAppExtraEntriesInfoFilePath(app), (UnauthorizedAccess e) =>
		{
		});
	}

	public DeleteAppExtraEntries(ReadAppExtraEntriesInfoDelegate readAppExtraEntriesInfo, CombinePathDelegate combinePath, DeleteDirRecursivelyIfExistsDelegate deleteDirRecursivelyIfExists, DeleteFileIfExistsDelegate deleteFileIfExists, GetAppExtraEntriesInfoFilePathDelegate getAppExtraEntriesInfoFilePath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(readAppExtraEntriesInfo);
		Assert.IsNotNull(combinePath);
		Assert.IsNotNull(deleteDirRecursivelyIfExists);
		Assert.IsNotNull(deleteFileIfExists);
		Assert.IsNotNull(getAppExtraEntriesInfoFilePath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ReadAppExtraEntriesInfo = readAppExtraEntriesInfo;
		CombinePath = combinePath;
		DeleteDirRecursivelyIfExists = deleteDirRecursivelyIfExists;
		DeleteFileIfExists = deleteFileIfExists;
		GetAppExtraEntriesInfoFilePath = getAppExtraEntriesInfoFilePath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing DeleteAppExtraEntries...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app);
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppExtraEntries finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeleteAppExtraEntries finished: UnauthorizedAccess.");
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
			LogInfo("DeleteAppExtraEntries has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppExtraEntries failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeleteAppExtraEntries finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeleteAppExtraEntriesDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeleteAppExtraEntries(context.Resolve<ReadAppExtraEntriesInfoDelegate>(), context.Resolve<CombinePathDelegate>(), context.Resolve<DeleteDirRecursivelyIfExistsDelegate>(), context.Resolve<DeleteFileIfExistsDelegate>(), context.Resolve<GetAppExtraEntriesInfoFilePathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
