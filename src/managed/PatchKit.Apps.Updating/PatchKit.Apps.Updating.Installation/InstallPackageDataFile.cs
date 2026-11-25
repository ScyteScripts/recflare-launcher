using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class InstallPackageDataFile
{
	public DeleteAppFileOrDirDelegate DeleteAppFileOrDir { get; }

	public CreateAppFileDelegate CreateAppFile { get; }

	public WritePackageFileStreamToFileStreamDelegate WritePackageFileStreamToFileStream { get; }

	public SetAppMetaDataEntryDelegate SetAppMetaDataEntry { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, InstallState installState, IPackageDataFile file, ReportProgressDelegate<object> reportProgress, CancellationToken cancellationToken)
	{
		DeleteAppFileOrDir(app, file.Path, null);
		IFileWriteStreamDisposable fileWriteStreamDisposable = CreateAppFile(app, file.Path, null, (ConflictingAppFile e) => new UnpredictedException(e), (ConflictingAppDir e) => new UnpredictedException(e));
		Assert.IsNotNullFromDelegate(fileWriteStreamDisposable);
		InstallPackageDataFileTask currentInstallPackageDataFileTask = new InstallPackageDataFileTask();
		installState.CurrentInstallPackageDataFileTask = currentInstallPackageDataFileTask;
		using (fileWriteStreamDisposable)
		{
			WritePackageFileStreamToFileStream(file.Stream, fileWriteStreamDisposable, ReportProgress, cancellationToken, null, null, null, null);
		}
		installState.CurrentInstallPackageDataFileTask = null;
		SetAppMetaDataEntry(app, file.Path, file.VersionId, null);
		void ReportProgress(BytesCount bytesCount)
		{
			reportProgress(null);
		}
	}

	public InstallPackageDataFile(DeleteAppFileOrDirDelegate deleteAppFileOrDir, CreateAppFileDelegate createAppFile, WritePackageFileStreamToFileStreamDelegate writePackageFileStreamToFileStream, SetAppMetaDataEntryDelegate setAppMetaDataEntry, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(deleteAppFileOrDir);
		Assert.IsNotNull(createAppFile);
		Assert.IsNotNull(writePackageFileStreamToFileStream);
		Assert.IsNotNull(setAppMetaDataEntry);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DeleteAppFileOrDir = deleteAppFileOrDir;
		CreateAppFile = createAppFile;
		WritePackageFileStreamToFileStream = writePackageFileStreamToFileStream;
		SetAppMetaDataEntry = setAppMetaDataEntry;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, InstallState installState, IPackageDataFile file, ReportProgressDelegate<object> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(installState, "installState");
		Arg.IsNotNull(file, "file");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing InstallPackageDataFile...");
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
			LogInfo("InstallPackageDataFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("InstallPackageDataFile finished: UnauthorizedAccess.");
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
			LogInfo("InstallPackageDataFile finished: OutOfFreeDiskSpace.");
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
			LogInfo("InstallPackageDataFile finished: InvalidAppAuthKey.");
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
			LogInfo("InstallPackageDataFile finished: ExpiredAppAuthKey.");
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
			LogInfo("InstallPackageDataFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("InstallPackageDataFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("InstallPackageDataFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public InstallPackageDataFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new InstallPackageDataFile(context.Resolve<DeleteAppFileOrDirDelegate>(), context.Resolve<CreateAppFileDelegate>(), context.Resolve<WritePackageFileStreamToFileStreamDelegate>(), context.Resolve<SetAppMetaDataEntryDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
