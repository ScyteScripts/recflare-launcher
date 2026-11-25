using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Librsync;

public class PatchFileOverwrite
{
	public PatchFileDelegate PatchFile { get; }

	public GetTempDirDelegate GetTempDir { get; }

	public MoveFileOverwriteDelegate MoveFileOverwrite { get; }

	public CombinePathDelegate CombinePath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path path, Path diffPath)
	{
		using ITempDirDisposable tempDirDisposable = GetTempDir(null);
		Assert.IsNotNullFromDelegate(tempDirDisposable);
		Path path2 = CombinePath(tempDirDisposable.Path, new Path("tempFile"));
		PatchFile(path, diffPath, path2, null, null, null, null, (ConflictingFile e) =>
		{
			throw new UnpredictedException(e);
		}, (ConflictingDir e) =>
		{
			throw new UnpredictedException(e);
		});
		MoveFileOverwrite(path2, path, null, (NotExistingFile e) =>
		{
			throw new UnpredictedException(e);
		}, (ConflictingDir e) =>
		{
			throw new UnpredictedException(e);
		});
	}

	public PatchFileOverwrite(PatchFileDelegate patchFile, GetTempDirDelegate getTempDir, MoveFileOverwriteDelegate moveFileOverwrite, CombinePathDelegate combinePath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(patchFile);
		Assert.IsNotNull(getTempDir);
		Assert.IsNotNull(moveFileOverwrite);
		Assert.IsNotNull(combinePath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		PatchFile = patchFile;
		GetTempDir = getTempDir;
		MoveFileOverwrite = moveFileOverwrite;
		CombinePath = combinePath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path path, Path diffPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<FilePatchingFailure> onFilePatchingFailure, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<NotExistingDiffFile> onNotExistingDiffFile)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(diffPath, "diffPath");
		LogInfo("Executing PatchFileOverwrite...");
		LogTrace("path = " + path);
		LogTrace("diffPath = " + diffPath);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(path, diffPath);
			EndLogRegion(enabled: true);
			LogInfo("PatchFileOverwrite finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFileOverwrite finished: UnauthorizedAccess.");
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
			LogInfo("PatchFileOverwrite finished: FilePatchingFailure.");
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
			LogInfo("PatchFileOverwrite finished: NotExistingFile.");
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
			LogInfo("PatchFileOverwrite finished: NotExistingDiffFile.");
			if (onNotExistingDiffFile == null)
			{
				throw;
			}
			onNotExistingDiffFile(e4);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchFileOverwrite has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("PatchFileOverwrite failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("PatchFileOverwrite finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public PatchFileOverwriteDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new PatchFileOverwrite(context.Resolve<PatchFileDelegate>(), context.Resolve<GetTempDirDelegate>(), context.Resolve<MoveFileOverwriteDelegate>(), context.Resolve<CombinePathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
