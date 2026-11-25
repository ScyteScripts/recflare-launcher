using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class DoesAppNeedRepair
{
	public DoesAppRootDirExistDelegate DoesAppRootDirExist { get; }

	public ReadAppMetaDataDelegate ReadAppMetaData { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(App app)
	{
		if (!DoesAppRootDirExist(app, null))
		{
			throw new AppRootDirDoesNotExist();
		}
		AppMetaData appMetaData = ReadAppMetaData(app, (UnauthorizedAccess e) =>
		{
			throw new UnauthorizedAccessToAppRootDir();
		});
		AppLastFilesOperationTaskState? lastFilesOperationTaskState = appMetaData.Info.LastFilesOperationTaskState;
		if (lastFilesOperationTaskState.HasValue)
		{
			AppLastInstallTaskState? appLastInstallTaskState = lastFilesOperationTaskState.GetValueOrDefault().AppLastInstallTaskState;
			if (appLastInstallTaskState.HasValue && appLastInstallTaskState.GetValueOrDefault().Error.HasValue)
			{
				goto IL_014e;
			}
		}
		lastFilesOperationTaskState = appMetaData.Info.LastFilesOperationTaskState;
		if (lastFilesOperationTaskState.HasValue)
		{
			AppLastUpdateTaskState? appLastUpdateTaskState = lastFilesOperationTaskState.GetValueOrDefault().AppLastUpdateTaskState;
			if (appLastUpdateTaskState.HasValue && appLastUpdateTaskState.GetValueOrDefault().Error.HasValue)
			{
				goto IL_014e;
			}
		}
		lastFilesOperationTaskState = appMetaData.Info.LastFilesOperationTaskState;
		int num = ((lastFilesOperationTaskState.HasValue && (lastFilesOperationTaskState.GetValueOrDefault().AppLastRepairTaskState?.Error.HasValue ?? false)) ? 1 : 0);
		goto IL_014f;
		IL_014e:
		num = 1;
		goto IL_014f;
		IL_014f:
		bool flag = (byte)num != 0;
		if (!appMetaData.Info.LastFilesOperationTaskState.HasValue || !flag)
		{
			if (!appMetaData.Info.InstalledVersionId.HasValue)
			{
				if (appMetaData.Entries.Length > 0)
				{
					return true;
				}
			}
			else if (!appMetaData.Info.StartInfo.HasValue)
			{
				return true;
			}
		}
		else if (appMetaData.Info.LastFilesOperationTaskState.Value.AppLastRepairTaskState.HasValue)
		{
			return true;
		}
		return false;
	}

	public DoesAppNeedRepair(DoesAppRootDirExistDelegate doesAppRootDirExist, ReadAppMetaDataDelegate readAppMetaData, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppRootDirExist);
		Assert.IsNotNull(readAppMetaData);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppRootDirExist = doesAppRootDirExist;
		ReadAppMetaData = readAppMetaData;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(App app, HandleExceptionAndReturnDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing DoesAppNeedRepair...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(app);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("DoesAppNeedRepair finished: success.");
			return result;
		}
		catch (AppRootDirDoesNotExist e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesAppNeedRepair finished: AppRootDirDoesNotExist.");
			if (onAppRootDirDoesNotExist == null)
			{
				throw;
			}
			object obj = onAppRootDirDoesNotExist(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (UnauthorizedAccessToAppRootDir e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesAppNeedRepair finished: UnauthorizedAccessToAppRootDir.");
			if (onUnauthorizedAccessToAppRootDir == null)
			{
				throw;
			}
			object obj2 = onUnauthorizedAccessToAppRootDir(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesAppNeedRepair has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DoesAppNeedRepair failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DoesAppNeedRepair finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DoesAppNeedRepairDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DoesAppNeedRepair(context.Resolve<DoesAppRootDirExistDelegate>(), context.Resolve<ReadAppMetaDataDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
