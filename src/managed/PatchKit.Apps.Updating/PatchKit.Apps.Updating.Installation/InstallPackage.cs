using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class InstallPackage
{
	public InstallPackageFileDelegate InstallPackageFile { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, InstallState installState, IPackage package, ReportProgressDelegate<InstallPackageProgress> reportProgress, CancellationToken cancellationToken)
	{
		bool flag = true;
		do
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPackageFileDisposable nextFile = package.GetNextFile(cancellationToken, null, null);
			if (nextFile == null)
			{
				flag = false;
				continue;
			}
			using (nextFile)
			{
				BytesCount startProcessedBytes = package.GetProcessedBytes();
				InstallPackageFile(app, installState, nextFile, ReportProgress, cancellationToken, null, null, null, null);
				void ReportProgress(object _)
				{
					BytesCount processedBytes = package.GetProcessedBytes();
					reportProgress(new InstallPackageProgress(new BytesCount(Math.Max(0L, processedBytes.Value - startProcessedBytes.Value))));
				}
			}
		}
		while (flag);
	}

	public InstallPackage(InstallPackageFileDelegate installPackageFile, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(installPackageFile);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		InstallPackageFile = installPackageFile;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, InstallState installState, IPackage package, ReportProgressDelegate<InstallPackageProgress> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(installState, "installState");
		Arg.IsNotNull(package, "package");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing InstallPackage...");
		LogTrace("app = " + app);
		LogTrace("installState = " + installState);
		LogTrace("package = " + package);
		LogTrace("reportProgress = " + reportProgress);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, installState, package, reportProgress, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("InstallPackage finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("InstallPackage finished: UnauthorizedAccess.");
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
			LogInfo("InstallPackage finished: OutOfFreeDiskSpace.");
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
			LogInfo("InstallPackage finished: InvalidAppAuthKey.");
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
			LogInfo("InstallPackage finished: ExpiredAppAuthKey.");
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
			LogInfo("InstallPackage has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("InstallPackage failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("InstallPackage finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public InstallPackageDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new InstallPackage(context.Resolve<InstallPackageFileDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
