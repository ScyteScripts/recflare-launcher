using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class WritePackageFileStreamToFile
{
	public CreateFileDelegate CreateFile { get; }

	public WritePackageFileStreamToFileStreamDelegate WritePackageFileStreamToFileStream { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(IPackageFileStream packageFileStream, Path filePath, ReportProgressDelegate<BytesCount> reportProgress, CancellationToken cancellationToken)
	{
		IFileWriteStreamDisposable fileWriteStreamDisposable = CreateFile(filePath, null, null, null);
		Assert.IsNotNullFromDelegate(fileWriteStreamDisposable);
		using (fileWriteStreamDisposable)
		{
			WritePackageFileStreamToFileStream(packageFileStream, fileWriteStreamDisposable, reportProgress, cancellationToken, null, null, null, null);
		}
	}

	public WritePackageFileStreamToFile(CreateFileDelegate createFile, WritePackageFileStreamToFileStreamDelegate writePackageFileStreamToFileStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createFile);
		Assert.IsNotNull(writePackageFileStreamToFileStream);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateFile = createFile;
		WritePackageFileStreamToFileStream = writePackageFileStreamToFileStream;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(IPackageFileStream packageFileStream, Path filePath, ReportProgressDelegate<BytesCount> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(packageFileStream, "packageFileStream");
		Arg.IsValid(filePath, "filePath");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing WritePackageFileStreamToFile...");
		LogTrace("packageFileStream = " + packageFileStream);
		LogTrace("filePath = " + filePath);
		LogTrace("reportProgress = " + reportProgress);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(packageFileStream, filePath, reportProgress, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (ConflictingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile finished: ConflictingFile.");
			if (onConflictingFile == null)
			{
				throw;
			}
			onConflictingFile(e2);
			return null;
		}
		catch (ConflictingDir e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile finished: ConflictingDir.");
			if (onConflictingDir == null)
			{
				throw;
			}
			onConflictingDir(e3);
			return null;
		}
		catch (OutOfFreeDiskSpace e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile finished: OutOfFreeDiskSpace.");
			if (onOutOfFreeDiskSpace == null)
			{
				throw;
			}
			onOutOfFreeDiskSpace(e4);
			return null;
		}
		catch (InvalidAppAuthKey e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			onInvalidAppAuthKey(e5);
			return null;
		}
		catch (ExpiredAppAuthKey e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			onExpiredAppAuthKey(e6);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("WritePackageFileStreamToFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("WritePackageFileStreamToFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public WritePackageFileStreamToFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new WritePackageFileStreamToFile(context.Resolve<CreateFileDelegate>(), context.Resolve<WritePackageFileStreamToFileStreamDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
