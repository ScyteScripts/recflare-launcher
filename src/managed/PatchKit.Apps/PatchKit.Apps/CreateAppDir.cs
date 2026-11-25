using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class CreateAppDir
{
	public CreateDirDelegate CreateDir { get; }

	public MoveDirOrFileToSafePlaceDelegate MoveDirOrFileToSafePlace { get; }

	public SetAppMetaDataEntryDelegate SetAppMetaDataEntry { get; }

	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path, AppVersionId versionId)
	{
		AppEntryInfo? appEntryInfo = GetAppEntryInfo(app, path, null);
		if (appEntryInfo.HasValue)
		{
			if (appEntryInfo.Value.Type == AppEntryType.Directory)
			{
				throw new ConflictingAppDir(app, path);
			}
			if (appEntryInfo.Value.Type == AppEntryType.File)
			{
				throw new ConflictingAppFile(app, path);
			}
		}
		Path path2 = GetAppEntryFullPath(app, path);
		MoveDirOrFileToSafePlace(path2, null);
		CreateDir(path2, null, (ConflictingFile e) =>
		{
			throw new UnpredictedException(e);
		});
		SetAppMetaDataEntry(app, path, versionId, null);
	}

	public CreateAppDir(CreateDirDelegate createDir, MoveDirOrFileToSafePlaceDelegate moveDirOrFileToSafePlace, SetAppMetaDataEntryDelegate setAppMetaDataEntry, GetAppEntryInfoDelegate getAppEntryInfo, GetAppEntryFullPathDelegate getAppEntryFullPath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createDir);
		Assert.IsNotNull(moveDirOrFileToSafePlace);
		Assert.IsNotNull(setAppMetaDataEntry);
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateDir = createDir;
		MoveDirOrFileToSafePlace = moveDirOrFileToSafePlace;
		SetAppMetaDataEntry = setAppMetaDataEntry;
		GetAppEntryInfo = getAppEntryInfo;
		GetAppEntryFullPath = getAppEntryFullPath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPath path, AppVersionId versionId, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingAppDir> onConflictingAppDir, HandleExceptionDelegate<ConflictingAppFile> onConflictingAppFile)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		Arg.IsValid(versionId, "versionId");
		LogInfo("Executing CreateAppDir...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path, versionId);
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDir finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDir finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (ConflictingAppDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDir finished: ConflictingAppDir.");
			if (onConflictingAppDir == null)
			{
				throw;
			}
			onConflictingAppDir(e2);
			return null;
		}
		catch (ConflictingAppFile e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDir finished: ConflictingAppFile.");
			if (onConflictingAppFile == null)
			{
				throw;
			}
			onConflictingAppFile(e3);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDir has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppDir failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppDir finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CreateAppDirDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CreateAppDir(context.Resolve<CreateDirDelegate>(), context.Resolve<MoveDirOrFileToSafePlaceDelegate>(), context.Resolve<SetAppMetaDataEntryDelegate>(), context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
