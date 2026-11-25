using System;
using System.Runtime.InteropServices;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Librsync;

public class PatchFile
{
	private delegate int NativePatchDelegate(string basisFile, string deltaFile, string newFile);

	public GetCurrentPlatformTypeDelegate GetCurrentPlatformType { get; }

	public DoesFileExistDelegate DoesFileExist { get; }

	public DoesDirExistDelegate DoesDirExist { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path path, Path diffPath, Path targetPath)
	{
		if (!DoesFileExist(path, null))
		{
			throw new NotExistingFile(path);
		}
		if (!DoesFileExist(diffPath, null))
		{
			throw new NotExistingDiffFile(diffPath);
		}
		if (DoesFileExist(targetPath, null))
		{
			throw new ConflictingFile(targetPath);
		}
		if (DoesDirExist(targetPath, null))
		{
			throw new ConflictingDir(targetPath);
		}
        Recovered.ManagedDelta.Apply(path, diffPath, targetPath);
    }

	public PatchFile(GetCurrentPlatformTypeDelegate getCurrentPlatformType, DoesFileExistDelegate doesFileExist, DoesDirExistDelegate doesDirExist, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getCurrentPlatformType);
		Assert.IsNotNull(doesFileExist);
		Assert.IsNotNull(doesDirExist);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetCurrentPlatformType = getCurrentPlatformType;
		DoesFileExist = doesFileExist;
		DoesDirExist = doesDirExist;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path path, Path diffPath, Path targetPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<FilePatchingFailure> onFilePatchingFailure, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<NotExistingDiffFile> onNotExistingDiffFile, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(diffPath, "diffPath");
		Arg.IsValid(targetPath, "targetPath");
		LogInfo("Executing PatchFile...");
		LogTrace("path = " + path);
		LogTrace("diffPath = " + diffPath);
		LogTrace("targetPath = " + targetPath);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(path, diffPath, targetPath);
			EndLogRegion(enabled: true);
			LogInfo("PatchFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (FilePatchingFailure e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFile finished: FilePatchingFailure.");
			if (onFilePatchingFailure == null)
			{
				throw;
			}
			onFilePatchingFailure(e2);
			return null;
		}
		catch (NotExistingFile e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFile finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			onNotExistingFile(e3);
			return null;
		}
		catch (NotExistingDiffFile e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFile finished: NotExistingDiffFile.");
			if (onNotExistingDiffFile == null)
			{
				throw;
			}
			onNotExistingDiffFile(e4);
			return null;
		}
		catch (ConflictingFile e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFile finished: ConflictingFile.");
			if (onConflictingFile == null)
			{
				throw;
			}
			onConflictingFile(e5);
			return null;
		}
		catch (ConflictingDir e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFile finished: ConflictingDir.");
			if (onConflictingDir == null)
			{
				throw;
			}
			onConflictingDir(e6);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("PatchFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("PatchFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public PatchFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new PatchFile(context.Resolve<GetCurrentPlatformTypeDelegate>(), context.Resolve<DoesFileExistDelegate>(), context.Resolve<DoesDirExistDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
