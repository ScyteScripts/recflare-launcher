using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Librsync;

namespace PatchKit.Apps.Updating.Installation;

public class ProcessInstallPackagePatchFileTasksQueue
{
	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public PatchFileDelegate PatchFile { get; }

	public MoveFileOverwriteDelegate MoveFileOverwrite { get; }

	public GetFileSizeDelegate GetFileSize { get; }

	public SetAppMetaDataEntryDelegate SetAppMetaDataEntry { get; }

	public DeleteAppFileDelegate DeleteAppFile { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, InstallState installState, CancellationToken cancellationToken)
	{
		if (installState.CurrentInstallPackagePatchFileTask != null)
		{
			InstallPackagePatchFileTask currentInstallPackagePatchFileTask = installState.CurrentInstallPackagePatchFileTask;
			currentInstallPackagePatchFileTask.FileProcessedBytesCount = GetFileSize(currentInstallPackagePatchFileTask.PatchedFilePath, (UnauthorizedAccess e) => new BytesCount(0L), (NotExistingFile e) => new BytesCount(0L));
			if (currentInstallPackagePatchFileTask.IsPatchingFinished)
			{
				if (currentInstallPackagePatchFileTask.PatchingException != null)
				{
					currentInstallPackagePatchFileTask.TempDir.Dispose();
					if (currentInstallPackagePatchFileTask.PatchingException is UnauthorizedAccess)
					{
						throw currentInstallPackagePatchFileTask.PatchingException;
					}
					if (currentInstallPackagePatchFileTask.PatchingException is FilePatchingFailure)
					{
						DeleteAppFile(app, currentInstallPackagePatchFileTask.FilePath, null, (NotExistingAppFile e) =>
						{
							throw new UnpredictedException(e);
						});
						throw new InconsistentInstallInfo(currentInstallPackagePatchFileTask.PatchingException);
					}
					throw new UnpredictedException(currentInstallPackagePatchFileTask.PatchingException);
				}
				Path newPath = GetAppEntryFullPath(app, currentInstallPackagePatchFileTask.FilePath);
				MoveFileOverwrite(currentInstallPackagePatchFileTask.PatchedFilePath, newPath, null, null, null);
				currentInstallPackagePatchFileTask.TempDir.Dispose();
				SetAppMetaDataEntry(app, currentInstallPackagePatchFileTask.FilePath, currentInstallPackagePatchFileTask.VersionPatchId.TargetVersionId, null);
				installState.FilesProcessedBytesCount = new BytesCount(installState.FilesProcessedBytesCount + currentInstallPackagePatchFileTask.FileProcessedBytesCount);
				installState.CurrentInstallPackagePatchFileTask = null;
			}
		}
		if (installState.CurrentInstallPackagePatchFileTask != null)
		{
			return;
		}
		InstallPackagePatchFileTask currentTask = ((installState.InstallPackagePatchFileTasksQueue.Count > 0) ? installState.InstallPackagePatchFileTasksQueue.Dequeue() : null);
		if (currentTask == null)
		{
			return;
		}
		installState.CurrentInstallPackagePatchFileTask = currentTask;
		AppEntryInfo? appEntryInfo = GetAppEntryInfo(app, currentTask.FilePath, null);
		if (!appEntryInfo.HasValue || appEntryInfo.Value.Type == AppEntryType.Directory)
		{
			throw new InconsistentInstallInfo();
		}
		if ((int)appEntryInfo.Value.VersionId != (int)currentTask.VersionPatchId.SourceVersionId)
		{
		}
		Path fullPath = GetAppEntryFullPath(app, currentTask.FilePath);
		ThreadPool.QueueUserWorkItem((object _) =>
		{
			try
			{
				PatchFile(fullPath, currentTask.PatchFilePath, currentTask.PatchedFilePath, null, null, null, null, null, null);
			}
			catch (Exception patchingException)
			{
				currentTask.PatchingException = patchingException;
			}
			finally
			{
				currentTask.IsPatchingFinished = true;
			}
		});
	}

	public ProcessInstallPackagePatchFileTasksQueue(GetAppEntryInfoDelegate getAppEntryInfo, GetAppEntryFullPathDelegate getAppEntryFullPath, PatchFileDelegate patchFile, MoveFileOverwriteDelegate moveFileOverwrite, GetFileSizeDelegate getFileSize, SetAppMetaDataEntryDelegate setAppMetaDataEntry, DeleteAppFileDelegate deleteAppFile, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(patchFile);
		Assert.IsNotNull(moveFileOverwrite);
		Assert.IsNotNull(getFileSize);
		Assert.IsNotNull(setAppMetaDataEntry);
		Assert.IsNotNull(deleteAppFile);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryInfo = getAppEntryInfo;
		GetAppEntryFullPath = getAppEntryFullPath;
		PatchFile = patchFile;
		MoveFileOverwrite = moveFileOverwrite;
		GetFileSize = getFileSize;
		SetAppMetaDataEntry = setAppMetaDataEntry;
		DeleteAppFile = deleteAppFile;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, InstallState installState, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<InconsistentInstallInfo> onInconsistentInstallInfo)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(installState, "installState");
		LogInfo("Executing ProcessInstallPackagePatchFileTasksQueue...");
		LogTrace("app = " + app);
		LogTrace("installState = " + installState);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, installState, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("ProcessInstallPackagePatchFileTasksQueue finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("ProcessInstallPackagePatchFileTasksQueue finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (InconsistentInstallInfo e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("ProcessInstallPackagePatchFileTasksQueue finished: InconsistentInstallInfo.");
			if (onInconsistentInstallInfo == null)
			{
				throw;
			}
			onInconsistentInstallInfo(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("ProcessInstallPackagePatchFileTasksQueue has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("ProcessInstallPackagePatchFileTasksQueue failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("ProcessInstallPackagePatchFileTasksQueue finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ProcessInstallPackagePatchFileTasksQueueDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ProcessInstallPackagePatchFileTasksQueue(context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<PatchFileDelegate>(), context.Resolve<MoveFileOverwriteDelegate>(), context.Resolve<GetFileSizeDelegate>(), context.Resolve<SetAppMetaDataEntryDelegate>(), context.Resolve<DeleteAppFileDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
