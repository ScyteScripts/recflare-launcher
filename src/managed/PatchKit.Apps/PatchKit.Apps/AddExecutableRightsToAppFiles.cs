using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class AddExecutableRightsToAppFiles
{
	public AddAppFileExecutableRightsIfRequiredDelegate AddAppFileExecutableRightsIfRequired { get; }

	public GetAppFilePathsDelegate GetAppFilePaths { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app)
	{
		foreach (AppPath item in GetAppFilePaths(app, null))
		{
			AddAppFileExecutableRightsIfRequired(app, item, null, (NotExistingAppFile e) =>
			{
				throw new UnpredictedException(e);
			});
		}
	}

	public AddExecutableRightsToAppFiles(AddAppFileExecutableRightsIfRequiredDelegate addAppFileExecutableRightsIfRequired, GetAppFilePathsDelegate getAppFilePaths, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(addAppFileExecutableRightsIfRequired);
		Assert.IsNotNull(getAppFilePaths);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		AddAppFileExecutableRightsIfRequired = addAppFileExecutableRightsIfRequired;
		GetAppFilePaths = getAppFilePaths;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing AddExecutableRightsToAppFiles...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app);
			EndLogRegion(enabled: true);
			LogInfo("AddExecutableRightsToAppFiles finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("AddExecutableRightsToAppFiles finished: UnauthorizedAccess.");
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
			LogInfo("AddExecutableRightsToAppFiles has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("AddExecutableRightsToAppFiles failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("AddExecutableRightsToAppFiles finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public AddExecutableRightsToAppFilesDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new AddExecutableRightsToAppFiles(context.Resolve<AddAppFileExecutableRightsIfRequiredDelegate>(), context.Resolve<GetAppFilePathsDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
