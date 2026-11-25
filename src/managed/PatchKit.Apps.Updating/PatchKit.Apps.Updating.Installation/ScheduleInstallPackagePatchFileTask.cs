using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class ScheduleInstallPackagePatchFileTask
{
	public GetTempDirDelegate GetTempDir { get; }

	public WritePackageFileStreamToFileDelegate WritePackageFileStreamToFile { get; }

	public PatchAppFileDelegate PatchAppFile { get; }

	public DeleteAppFileDelegate DeleteAppFile { get; }

	public CombinePathDelegate CombinePath { get; }

	public SetAppMetaDataEntryDelegate SetAppMetaDataEntry { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, InstallState installState, IPackagePatchFile file, ReportProgressDelegate<object> reportProgress, CancellationToken cancellationToken)
	{
		if (file.IsPatchEmpty)
		{
			SetAppMetaDataEntry(app, file.Path, file.TargetVersionId, null);
			return;
		}
		ITempDirDisposable tempDirDisposable = GetTempDir(null);
		Assert.IsNotNullFromDelegate(tempDirDisposable);
		try
		{
			Path path = CombinePath(tempDirDisposable.Path, new Path("patch"));
			WritePackageFileStreamToFileDelegate writePackageFileStreamToFile = WritePackageFileStreamToFile;
			Path filePath = path;
			writePackageFileStreamToFile(file.Stream, filePath, ReportProgress, cancellationToken, null, (ConflictingFile e) =>
			{
				throw new UnpredictedException(e);
			}, (ConflictingDir e) =>
			{
				throw new UnpredictedException(e);
			}, null, null, null);
			Path patchedFilePath = CombinePath(tempDirDisposable.Path, new Path("patched"));
			installState.InstallPackagePatchFileTasksQueue.Enqueue(new InstallPackagePatchFileTask(file.Path, new AppVersionPatchId(file.SourceVersionId, file.TargetVersionId), path, patchedFilePath, tempDirDisposable));
		}
		catch
		{
			tempDirDisposable.Dispose();
			throw;
		}
		void ReportProgress(BytesCount _)
		{
			reportProgress(null);
		}
	}

	public ScheduleInstallPackagePatchFileTask(GetTempDirDelegate getTempDir, WritePackageFileStreamToFileDelegate writePackageFileStreamToFile, PatchAppFileDelegate patchAppFile, DeleteAppFileDelegate deleteAppFile, CombinePathDelegate combinePath, SetAppMetaDataEntryDelegate setAppMetaDataEntry, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getTempDir);
		Assert.IsNotNull(writePackageFileStreamToFile);
		Assert.IsNotNull(patchAppFile);
		Assert.IsNotNull(deleteAppFile);
		Assert.IsNotNull(combinePath);
		Assert.IsNotNull(setAppMetaDataEntry);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetTempDir = getTempDir;
		WritePackageFileStreamToFile = writePackageFileStreamToFile;
		PatchAppFile = patchAppFile;
		DeleteAppFile = deleteAppFile;
		CombinePath = combinePath;
		SetAppMetaDataEntry = setAppMetaDataEntry;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, InstallState installState, IPackagePatchFile file, ReportProgressDelegate<object> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(installState, "installState");
		Arg.IsNotNull(file, "file");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing ScheduleInstallPackagePatchFileTask...");
		LogTrace("app = " + app);
		LogTrace("installState = " + installState);
		LogTrace("file = " + file);
		LogTrace("reportProgress = " + reportProgress);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, installState, file, reportProgress, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("ScheduleInstallPackagePatchFileTask finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("ScheduleInstallPackagePatchFileTask finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (OutOfFreeDiskSpace e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("ScheduleInstallPackagePatchFileTask finished: OutOfFreeDiskSpace.");
			if (onOutOfFreeDiskSpace == null)
			{
				throw;
			}
			onOutOfFreeDiskSpace(e2);
			return null;
		}
		catch (InvalidAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("ScheduleInstallPackagePatchFileTask finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			onInvalidAppAuthKey(e3);
			return null;
		}
		catch (ExpiredAppAuthKey e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("ScheduleInstallPackagePatchFileTask finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			onExpiredAppAuthKey(e4);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("ScheduleInstallPackagePatchFileTask has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("ScheduleInstallPackagePatchFileTask failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("ScheduleInstallPackagePatchFileTask finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ScheduleInstallPackagePatchFileTaskDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ScheduleInstallPackagePatchFileTask(context.Resolve<GetTempDirDelegate>(), context.Resolve<WritePackageFileStreamToFileDelegate>(), context.Resolve<PatchAppFileDelegate>(), context.Resolve<DeleteAppFileDelegate>(), context.Resolve<CombinePathDelegate>(), context.Resolve<SetAppMetaDataEntryDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
