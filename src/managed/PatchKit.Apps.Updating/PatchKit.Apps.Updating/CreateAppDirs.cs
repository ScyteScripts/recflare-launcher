using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class CreateAppDirs
{
	public DeleteAppFileOrDirDelegate DeleteAppFileOrDir { get; }

	public CreateAppDirDelegate CreateAppDir { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppVersionId versionId, ImmutableArray<AppPath> dirs)
	{
		foreach (AppPath item in dirs)
		{
			DeleteAppFileOrDir(app, item, null);
			CreateAppDir(app, item, versionId, null, (ConflictingAppDir e) =>
			{
				throw new UnpredictedException(e);
			}, (ConflictingAppFile e) =>
			{
				throw new UnpredictedException(e);
			});
		}
	}

	public CreateAppDirs(DeleteAppFileOrDirDelegate deleteAppFileOrDir, CreateAppDirDelegate createAppDir, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(deleteAppFileOrDir);
		Assert.IsNotNull(createAppDir);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DeleteAppFileOrDir = deleteAppFileOrDir;
		CreateAppDir = createAppDir;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppVersionId versionId, ImmutableArray<AppPath> dirs, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(versionId, "versionId");
		Arg.IsValid(dirs, "dirs");
		LogInfo("Executing CreateAppDirs...");
		LogTrace("app = " + app);
		AppVersionId appVersionId = versionId;
		LogTrace("versionId = " + appVersionId.ToString());
		ImmutableArray<AppPath> immutableArray = dirs;
		LogTrace("dirs = " + immutableArray.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, versionId, dirs);
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDirs finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDirs finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("CreateAppDirs has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppDirs failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("CreateAppDirs finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public CreateAppDirsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new CreateAppDirs(context.Resolve<DeleteAppFileOrDirDelegate>(), context.Resolve<CreateAppDirDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
