using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class CreateAppFile
{
	public CreateFileDelegate CreateFile { get; }

	public MoveDirOrFileToSafePlaceDelegate MoveDirOrFileToSafePlace { get; }

	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IFileWriteStreamDisposable Execute(App app, AppPath path)
	{
		AppEntryInfo? appEntryInfo = GetAppEntryInfo(app, path, null);
		if (appEntryInfo.HasValue)
		{
			if (appEntryInfo.Value.Type == AppEntryType.File)
			{
				throw new ConflictingAppFile(app, path);
			}
			if (appEntryInfo.Value.Type == AppEntryType.Directory)
			{
				throw new ConflictingAppDir(app, path);
			}
		}
		Path path2 = GetAppEntryFullPath(app, path);
		MoveDirOrFileToSafePlace(path2, null);
		IFileWriteStreamDisposable fileWriteStreamDisposable = CreateFile(path2, null, (ConflictingFile e) =>
		{
			throw new UnpredictedException(e);
		}, (ConflictingDir e) =>
		{
			throw new UnpredictedException(e);
		});
		Assert.IsNotNullFromDelegate(fileWriteStreamDisposable);
		return fileWriteStreamDisposable;
	}

	public CreateAppFile(CreateFileDelegate createFile, MoveDirOrFileToSafePlaceDelegate moveDirOrFileToSafePlace, GetAppEntryInfoDelegate getAppEntryInfo, GetAppEntryFullPathDelegate getAppEntryFullPath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createFile);
		Assert.IsNotNull(moveDirOrFileToSafePlace);
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateFile = createFile;
		MoveDirOrFileToSafePlace = moveDirOrFileToSafePlace;
		GetAppEntryInfo = getAppEntryInfo;
		GetAppEntryFullPath = getAppEntryFullPath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IFileWriteStreamDisposable DelegateExecute(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<ConflictingAppFile> onConflictingAppFile, HandleExceptionAndReturnDelegate<ConflictingAppDir> onConflictingAppDir)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing CreateAppFile...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			IFileWriteStreamDisposable fileWriteStreamDisposable = Execute(app, path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + fileWriteStreamDisposable);
			LogInfo("CreateAppFile finished: success.");
			return fileWriteStreamDisposable;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (IFileWriteStreamDisposable)obj;
		}
		catch (ConflictingAppFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppFile finished: ConflictingAppFile.");
			if (onConflictingAppFile == null)
			{
				throw;
			}
			object obj2 = onConflictingAppFile(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (IFileWriteStreamDisposable)obj2;
		}
		catch (ConflictingAppDir e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppFile finished: ConflictingAppDir.");
			if (onConflictingAppDir == null)
			{
				throw;
			}
			object obj3 = onConflictingAppDir(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (IFileWriteStreamDisposable)obj3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CreateAppFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CreateAppFile(context.Resolve<CreateFileDelegate>(), context.Resolve<MoveDirOrFileToSafePlaceDelegate>(), context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
