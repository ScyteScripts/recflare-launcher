using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class InstallPackageFile
{
	public InstallPackageDataFileDelegate InstallPackageDataFile { get; }

	public ScheduleInstallPackagePatchFileTaskDelegate ScheduleInstallPackagePatchFileTask { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, InstallState installState, IPackageFile file, ReportProgressDelegate<object> reportProgress, CancellationToken cancellationToken)
	{
		if (file is IPackageDataFile file2)
		{
			InstallPackageDataFile(app, installState, file2, reportProgress, cancellationToken, null, null, null, null);
			return;
		}
		if (file is IPackagePatchFile file3)
		{
			ScheduleInstallPackagePatchFileTask(app, installState, file3, reportProgress, cancellationToken, null, null, null, null);
			return;
		}
		throw new UnpredictedCondition("Unknown package file type.");
	}

	public InstallPackageFile(InstallPackageDataFileDelegate installPackageDataFile, ScheduleInstallPackagePatchFileTaskDelegate scheduleInstallPackagePatchFileTask, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(installPackageDataFile);
		Assert.IsNotNull(scheduleInstallPackagePatchFileTask);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		InstallPackageDataFile = installPackageDataFile;
		ScheduleInstallPackagePatchFileTask = scheduleInstallPackagePatchFileTask;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, InstallState installState, IPackageFile file, ReportProgressDelegate<object> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(installState, "installState");
		Arg.IsNotNull(file, "file");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing InstallPackageFile...");
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
			LogInfo("InstallPackageFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("InstallPackageFile finished: UnauthorizedAccess.");
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
			LogInfo("InstallPackageFile finished: OutOfFreeDiskSpace.");
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
			LogInfo("InstallPackageFile finished: InvalidAppAuthKey.");
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
			LogInfo("InstallPackageFile finished: ExpiredAppAuthKey.");
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
			LogInfo("InstallPackageFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("InstallPackageFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("InstallPackageFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public InstallPackageFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new InstallPackageFile(context.Resolve<InstallPackageDataFileDelegate>(), context.Resolve<ScheduleInstallPackagePatchFileTaskDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
