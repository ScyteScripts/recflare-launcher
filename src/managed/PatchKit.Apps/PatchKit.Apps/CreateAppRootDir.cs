using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class CreateAppRootDir
{
	public CreateDirDelegate CreateDir { get; }

	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public IsDirEmptyDelegate IsDirEmpty { get; }

	public GetDefaultAppMetaDataDelegate GetDefaultAppMetaData { get; }

	public WriteAppMetaDataCacheDelegate WriteAppMetaDataCache { get; }

	public SetAppRootDirPermissionsDelegate SetAppRootDirPermissions { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, bool isOverwritingOtherAppRootDirAllowed, bool isAppNotEmptyRootDirAllowed)
	{
		CreateDir(app.Path, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}, (ConflictingFile e) =>
		{
			throw new FileIsConflictingWithAppRootDir();
		});
		bool flag = DoesAppRootDirExist(app, null);
		bool flag2 = IsDirEmpty(app.Path, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		}, (NotExistingDir e) =>
		{
			throw new UnpredictedException(e);
		});
		if (flag)
		{
			if (!isOverwritingOtherAppRootDirAllowed)
			{
				throw new OtherAppRootDirIsConflictingWithAppRootDir();
			}
		}
		else if (!flag2 && !isAppNotEmptyRootDirAllowed)
		{
			throw new AppRootDirIsNotEmpty();
		}
		app.MetaDataCache = GetDefaultAppMetaData();
		WriteAppMetaDataCache(app, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
		SetAppRootDirPermissions(app, null);
	}

	public CreateAppRootDir(CreateDirDelegate createDir, DoesAppRootDirExistDelegate doesAppRootDirExist, IsDirEmptyDelegate isDirEmpty, GetDefaultAppMetaDataDelegate getDefaultAppMetaData, WriteAppMetaDataCacheDelegate writeAppMetaDataCache, SetAppRootDirPermissionsDelegate setAppRootDirPermissions, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(createDir);
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(isDirEmpty);
		Assert.IsNotNull(getDefaultAppMetaData);
		Assert.IsNotNull(writeAppMetaDataCache);
		Assert.IsNotNull(setAppRootDirPermissions);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CreateDir = createDir;
		DoesAppRootDirExist = doesAppRootDirExist;
		IsDirEmpty = isDirEmpty;
		GetDefaultAppMetaData = getDefaultAppMetaData;
		WriteAppMetaDataCache = writeAppMetaDataCache;
		SetAppRootDirPermissions = setAppRootDirPermissions;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, bool isOverwritingOtherAppRootDirAllowed, bool isAppNotEmptyRootDirAllowed, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir, HandleExceptionDelegate<FileIsConflictingWithAppRootDir> onFileIsConflictingWithAppRootDir, HandleExceptionDelegate<OtherAppRootDirIsConflictingWithAppRootDir> onOtherAppRootDirIsConflictingWithAppRootDir, HandleExceptionDelegate<AppRootDirIsNotEmpty> onAppRootDirIsNotEmpty)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing CreateAppRootDir...");
		LogTrace("app = " + app);
		LogTrace("isOverwritingOtherAppRootDirAllowed = " + isOverwritingOtherAppRootDirAllowed);
		LogTrace("isAppNotEmptyRootDirAllowed = " + isAppNotEmptyRootDirAllowed);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, isOverwritingOtherAppRootDirAllowed, isAppNotEmptyRootDirAllowed);
			EndLogRegion(enabled: true);
			LogInfo("CreateAppRootDir finished: success.");
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir e)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppRootDir finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			onUnauthorizedAccessToAppRootDir(e);
			return null;
		}
		catch (FileIsConflictingWithAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppRootDir finished: FileIsConflictingWithAppRootDir.");
			if (onFileIsConflictingWithAppRootDir == null)
			{
				throw;
			}
			onFileIsConflictingWithAppRootDir(e2);
			return null;
		}
		catch (OtherAppRootDirIsConflictingWithAppRootDir e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppRootDir finished: OtherAppRootDirIsConflictingWithAppRootDir.");
			if (onOtherAppRootDirIsConflictingWithAppRootDir == null)
			{
				throw;
			}
			onOtherAppRootDirIsConflictingWithAppRootDir(e3);
			return null;
		}
		catch (AppRootDirIsNotEmpty e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppRootDir finished: AppRootDirIsNotEmpty.");
			if (onAppRootDirIsNotEmpty == null)
			{
				throw;
			}
			onAppRootDirIsNotEmpty(e4);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppRootDir has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppRootDir failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppRootDir finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CreateAppRootDirDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CreateAppRootDir(context.Resolve<CreateDirDelegate>(), context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<IsDirEmptyDelegate>(), context.Resolve<GetDefaultAppMetaDataDelegate>(), context.Resolve<WriteAppMetaDataCacheDelegate>(), context.Resolve<SetAppRootDirPermissionsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
