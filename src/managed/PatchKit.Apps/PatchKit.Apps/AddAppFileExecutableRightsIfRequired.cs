using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class AddAppFileExecutableRightsIfRequired
{
	public AddFileExecutableRightsIfRequiredDelegate AddFileExecutableRightsIfRequired { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path)
	{
		Path path2 = GetAppEntryFullPath(app, path);
		AddFileExecutableRightsIfRequired(path2, null, (NotExistingFile e) =>
		{
			throw new NotExistingAppFile(app, path, e);
		});
	}

	public AddAppFileExecutableRightsIfRequired(AddFileExecutableRightsIfRequiredDelegate addFileExecutableRightsIfRequired, GetAppEntryFullPathDelegate getAppEntryFullPath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(addFileExecutableRightsIfRequired);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		AddFileExecutableRightsIfRequired = addFileExecutableRightsIfRequired;
		GetAppEntryFullPath = getAppEntryFullPath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingAppFile> onNotExistingAppFile)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing AddAppFileExecutableRightsIfRequired...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path);
			EndLogRegion(enabled: true);
			LogInfo("AddAppFileExecutableRightsIfRequired finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("AddAppFileExecutableRightsIfRequired finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (NotExistingAppFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("AddAppFileExecutableRightsIfRequired finished: NotExistingAppFile.");
			if (onNotExistingAppFile == null)
			{
				throw;
			}
			onNotExistingAppFile(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("AddAppFileExecutableRightsIfRequired has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("AddAppFileExecutableRightsIfRequired failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("AddAppFileExecutableRightsIfRequired finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public AddAppFileExecutableRightsIfRequiredDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new AddAppFileExecutableRightsIfRequired(context.Resolve<AddFileExecutableRightsIfRequiredDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
