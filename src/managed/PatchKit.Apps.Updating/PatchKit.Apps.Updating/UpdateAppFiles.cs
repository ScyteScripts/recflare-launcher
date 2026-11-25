using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Apps.Updating.Installation;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Apps.Updating.Strategy;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class UpdateAppFiles
{
	private struct BytesCountPerSecondEntry
	{
		public DateTime DateTime;

		public long PackagesProcessedBytesCount;

		public long FilesProcessedBytesCount;
	}

	public GetFileUpdateStrategiesPackagesInfoDelegate GetFileUpdateStrategiesPackagesInfo { get; }

	public GetDirFreeSpaceBytesCountDelegate GetDirFreeSpaceBytesCount { get; }

	public GetPackageDelegate GetPackage { get; }

	public InstallPackageDelegate InstallPackage { get; }

	public ProcessInstallPackagePatchFileTasksQueueDelegate ProcessInstallPackagePatchFileTasksQueue { get; }

	public WriteAppMetaDataCacheDelegate WriteAppMetaDataCache { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(UpdateAppContext context, ImmutableArray<FileUpdateStrategy> strategy, ReportProgressDelegate<UpdateAppProgress> reportProgress, CancellationToken cancellationToken)
	{
		if (strategy.Length == 0)
		{
			return;
		}
		ImmutableArray<AppRemotePackageInfo> immutableArray = GetFileUpdateStrategiesPackagesInfo(context.AppRemote, licenseSecret: context.LicenseSecret, password: context.Password, targetVersionId: context.TargetVersionId, strategies: strategy, cancellationToken: cancellationToken, onAppRemoteVersionNotFound: null, onInvalidAppPassword: null, onInvalidAppAuthKey: null, onExpiredAppAuthKey: null);
		BytesCount packagesTotalBytesCount = new BytesCount(immutableArray.Sum((AppRemotePackageInfo x) => x.Size));
		if (GetDirFreeSpaceBytesCount(context.App.Path, null).Value < packagesTotalBytesCount.Value)
		{
			throw new OutOfFreeDiskSpace();
		}
		BytesCount filesTotalBytesCount = new BytesCount((long)packagesTotalBytesCount + strategy.Sum((FileUpdateStrategy x) =>
		{
			if (x.DataStrategy.HasValue)
			{
				return 0L;
			}
			return x.PatchesStrategy.HasValue ? ((long)x.PatchesStrategy.Value.BytesCount) : 0;
		}));
		reportProgress(new UpdateAppProgress(null, new BytesCount(0L), packagesTotalBytesCount, new BytesCount(0L), new BytesCount(0L), filesTotalBytesCount, new BytesCount(0L)));
		List<BytesCountPerSecondEntry> bytesCountPerSecondEntries = new List<BytesCountPerSecondEntry>();
		BytesCountPerSecondEntry? lastBytesCountPerSecondEntry = null;
		long filesProcessedBytesCount = 0L;
		long finishedPackagesProcessedBytesCount = 0L;
		long packagesLastProcessedBytesCount = 0L;
		long filesLastProcessedBytesCount = 0L;
		long packagesProcessedBytesCountPerSecond = 0L;
		long filesProcessedBytesCountPerSecond = 0L;
		InstallState installState = new InstallState();
		try
		{
			foreach (AppRemotePackageInfo item in immutableArray)
			{
				cancellationToken.ThrowIfCancellationRequested();
				IPackageDisposable package = GetPackage(item);
				Assert.IsNotNullFromDelegate(package);
				using (package)
				{
					CancellationTokenSource installPackageCancellationTokenSource = new CancellationTokenSource();
					Exception processInstallPackagePatchFileTasksQueueException = null;
					try
					{
						InstallPackage(context.App, installState, package, ReportProgress, CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, installPackageCancellationTokenSource.Token).Token, null, null, null, null);
					}
					catch (OperationCanceledException)
					{
						if (processInstallPackagePatchFileTasksQueueException != null)
						{
							throw processInstallPackagePatchFileTasksQueueException;
						}
						throw;
					}
					WriteAppMetaDataCache(context.App, null);
					void ReportProgress(InstallPackageProgress installPackageProgress)
					{
						if (processInstallPackagePatchFileTasksQueueException == null)
						{
							try
							{
								ProcessInstallPackagePatchFileTasksQueue(context.App, installState, cancellationToken, null, (InconsistentInstallInfo e) =>
								{
									throw new UpdateAppFailure(e);
								});
							}
							catch (Exception ex2)
							{
								processInstallPackagePatchFileTasksQueueException = ex2;
								installPackageCancellationTokenSource.Cancel();
							}
							DateTime now2 = DateTime.Now;
							if (!lastBytesCountPerSecondEntry.HasValue || !((now2 - lastBytesCountPerSecondEntry.Value.DateTime).TotalSeconds < 1.0))
							{
								long num = finishedPackagesProcessedBytesCount + (long)package.GetProcessedBytes();
								UpdateFilesProcessedBytesCount(num);
								UpdateBytesCountPerSecond(now2, num);
								reportProgress(new UpdateAppProgress(installPackageProgress.CurrentFileProcessedBytesCount, new BytesCount(Math.Max(0L, num)), packagesTotalBytesCount, new BytesCount(Math.Max(0L, packagesProcessedBytesCountPerSecond)), new BytesCount(filesProcessedBytesCount), filesTotalBytesCount, new BytesCount(Math.Max(0L, filesProcessedBytesCountPerSecond))));
								WriteAppMetaDataCache(context.App, null);
							}
						}
					}
				}
				finishedPackagesProcessedBytesCount += (long)item.Size;
			}
			while (installState.CurrentInstallPackagePatchFileTask != null || installState.InstallPackagePatchFileTasksQueue.Count > 0)
			{
				ProcessInstallPackagePatchFileTasksQueue(context.App, installState, cancellationToken, null, (InconsistentInstallInfo e) =>
				{
					throw new UpdateAppFailure(e);
				});
				DateTime now = DateTime.Now;
				UpdateFilesProcessedBytesCount(packagesTotalBytesCount);
				UpdateBytesCountPerSecond(now, packagesTotalBytesCount);
				reportProgress(new UpdateAppProgress(new BytesCount(0L), packagesTotalBytesCount, packagesTotalBytesCount, new BytesCount(packagesProcessedBytesCountPerSecond), new BytesCount(filesProcessedBytesCount), filesTotalBytesCount, new BytesCount(filesProcessedBytesCountPerSecond)));
			}
		}
		finally
		{
			if (installState.CurrentInstallPackagePatchFileTask != null)
			{
				installState.CurrentInstallPackagePatchFileTask.TempDir.Dispose();
			}
			foreach (InstallPackagePatchFileTask item2 in installState.InstallPackagePatchFileTasksQueue)
			{
				item2.TempDir.Dispose();
			}
		}
		void UpdateBytesCountPerSecond(DateTime dateTimeNow, long packagesProcessedBytesCount)
		{
			bytesCountPerSecondEntries.RemoveAll((BytesCountPerSecondEntry x) => (dateTimeNow - x.DateTime).TotalSeconds > 5.0);
			lastBytesCountPerSecondEntry = new BytesCountPerSecondEntry
			{
				PackagesProcessedBytesCount = Math.Max(0L, packagesProcessedBytesCount - packagesLastProcessedBytesCount),
				FilesProcessedBytesCount = Math.Max(0L, filesProcessedBytesCount - filesLastProcessedBytesCount),
				DateTime = dateTimeNow
			};
			bytesCountPerSecondEntries.Add(lastBytesCountPerSecondEntry.Value);
			if (bytesCountPerSecondEntries.Count > 1)
			{
				TimeSpan timeSpan = bytesCountPerSecondEntries.Last().DateTime - bytesCountPerSecondEntries.First().DateTime;
				long num = bytesCountPerSecondEntries.Sum((BytesCountPerSecondEntry x) => x.PackagesProcessedBytesCount);
				long num2 = bytesCountPerSecondEntries.Sum((BytesCountPerSecondEntry x) => x.FilesProcessedBytesCount);
				packagesProcessedBytesCountPerSecond = Math.Max(0L, (long)((double)num / timeSpan.TotalSeconds));
				filesProcessedBytesCountPerSecond = Math.Max(0L, (long)((double)num2 / timeSpan.TotalSeconds));
			}
			packagesLastProcessedBytesCount = packagesProcessedBytesCount;
			filesLastProcessedBytesCount = filesProcessedBytesCount;
		}
		void UpdateFilesProcessedBytesCount(long packagesProcessedBytesCount)
		{
			filesProcessedBytesCount = packagesProcessedBytesCount + (long)installState.FilesProcessedBytesCount + (installState.CurrentInstallPackagePatchFileTask?.FileProcessedBytesCount.Value ?? 0);
		}
	}

	public UpdateAppFiles(GetFileUpdateStrategiesPackagesInfoDelegate getFileUpdateStrategiesPackagesInfo, GetDirFreeSpaceBytesCountDelegate getDirFreeSpaceBytesCount, GetPackageDelegate getPackage, InstallPackageDelegate installPackage, ProcessInstallPackagePatchFileTasksQueueDelegate processInstallPackagePatchFileTasksQueue, WriteAppMetaDataCacheDelegate writeAppMetaDataCache, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getFileUpdateStrategiesPackagesInfo);
		Assert.IsNotNull(getDirFreeSpaceBytesCount);
		Assert.IsNotNull(getPackage);
		Assert.IsNotNull(installPackage);
		Assert.IsNotNull(processInstallPackagePatchFileTasksQueue);
		Assert.IsNotNull(writeAppMetaDataCache);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetFileUpdateStrategiesPackagesInfo = getFileUpdateStrategiesPackagesInfo;
		GetDirFreeSpaceBytesCount = getDirFreeSpaceBytesCount;
		GetPackage = getPackage;
		InstallPackage = installPackage;
		ProcessInstallPackagePatchFileTasksQueue = processInstallPackagePatchFileTasksQueue;
		WriteAppMetaDataCache = writeAppMetaDataCache;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(UpdateAppContext context, ImmutableArray<FileUpdateStrategy> strategy, ReportProgressDelegate<UpdateAppProgress> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<UpdateAppFailure> onUpdateAppFailure, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsValid(context, "context");
		Arg.IsValid(strategy, "strategy");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing UpdateAppFiles...");
		UpdateAppContext updateAppContext = context;
		LogTrace("context = " + updateAppContext.ToString());
		ImmutableArray<FileUpdateStrategy> immutableArray = strategy;
		LogTrace("strategy = " + immutableArray.ToString());
		LogTrace("reportProgress = " + reportProgress);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(context, strategy, reportProgress, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (UpdateAppFailure e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: UpdateAppFailure.");
			if (onUpdateAppFailure == null)
			{
				throw;
			}
			onUpdateAppFailure(e2);
			return null;
		}
		catch (OutOfFreeDiskSpace e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: OutOfFreeDiskSpace.");
			if (onOutOfFreeDiskSpace == null)
			{
				throw;
			}
			onOutOfFreeDiskSpace(e3);
			return null;
		}
		catch (AppRemoteVersionNotFound e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			onAppRemoteVersionNotFound(e4);
			return null;
		}
		catch (InvalidAppPassword e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			onInvalidAppPassword(e5);
			return null;
		}
		catch (InvalidAppAuthKey e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			onInvalidAppAuthKey(e6);
			return null;
		}
		catch (ExpiredAppAuthKey e7)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			onExpiredAppAuthKey(e7);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppFiles has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("UpdateAppFiles failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("UpdateAppFiles finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public UpdateAppFilesDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new UpdateAppFiles(context.Resolve<GetFileUpdateStrategiesPackagesInfoDelegate>(), context.Resolve<GetDirFreeSpaceBytesCountDelegate>(), context.Resolve<GetPackageDelegate>(), context.Resolve<InstallPackageDelegate>(), context.Resolve<ProcessInstallPackagePatchFileTasksQueueDelegate>(), context.Resolve<WriteAppMetaDataCacheDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
