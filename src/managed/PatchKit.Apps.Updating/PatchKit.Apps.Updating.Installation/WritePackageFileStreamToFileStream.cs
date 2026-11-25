using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class WritePackageFileStreamToFileStream
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(IPackageFileStream packageFileStream, IFileWriteStream fileStream, ReportProgressDelegate<BytesCount> reportProgress, CancellationToken cancellationToken)
	{
		BufferLength length = new BufferLength(4194304);
		Buffer<byte> buffer = Buffer<byte>.CreateOfLength(length);
		BytesCount bytesCount = new BytesCount(0L);
		while (true)
		{
			StreamReadResult streamReadResult2;
			StreamReadResult streamReadResult = (streamReadResult2 = packageFileStream.Read(buffer, cancellationToken, null, null));
			if (!streamReadResult.IsNotEndOfStream)
			{
				break;
			}
			cancellationToken.ThrowIfCancellationRequested();
			Buffer<byte> buffer2 = buffer.ModifyLength(new BufferLength(streamReadResult2.Length));
			fileStream.Write(buffer2, null, null);
			bytesCount = new BytesCount((long)bytesCount + (int)streamReadResult2.Length);
			reportProgress(bytesCount);
		}
		reportProgress(bytesCount);
	}

	public WritePackageFileStreamToFileStream(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(IPackageFileStream packageFileStream, IFileWriteStream fileStream, ReportProgressDelegate<BytesCount> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(packageFileStream, "packageFileStream");
		Arg.IsNotNull(fileStream, "fileStream");
		Arg.IsNotNull(reportProgress, "reportProgress");
		LogInfo("Executing WritePackageFileStreamToFileStream...");
		LogTrace("packageFileStream = " + packageFileStream);
		LogTrace("fileStream = " + fileStream);
		LogTrace("reportProgress = " + reportProgress);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(packageFileStream, fileStream, reportProgress, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFileStream finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("WritePackageFileStreamToFileStream finished: UnauthorizedAccess.");
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
			LogInfo("WritePackageFileStreamToFileStream finished: OutOfFreeDiskSpace.");
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
			LogInfo("WritePackageFileStreamToFileStream finished: InvalidAppAuthKey.");
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
			LogInfo("WritePackageFileStreamToFileStream finished: ExpiredAppAuthKey.");
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
			LogInfo("WritePackageFileStreamToFileStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("WritePackageFileStreamToFileStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("WritePackageFileStreamToFileStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public WritePackageFileStreamToFileStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new WritePackageFileStreamToFileStream(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
