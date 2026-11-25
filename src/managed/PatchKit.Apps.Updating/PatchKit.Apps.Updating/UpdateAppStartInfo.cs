using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public class UpdateAppStartInfo
{
	public GetAppRemoteStartInfoDelegate GetAppRemoteStartInfo { get; }

	public SetAppStartInfoDelegate SetAppStartInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(UpdateAppContext context, CancellationToken cancellationToken)
	{
		AppStartInfo? value = GetAppRemoteStartInfo(context.AppRemote, context.TargetVersionId, cancellationToken, null);
		SetAppStartInfo(context.App, value, null);
	}

	public UpdateAppStartInfo(GetAppRemoteStartInfoDelegate getAppRemoteStartInfo, SetAppStartInfoDelegate setAppStartInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteStartInfo);
		Assert.IsNotNull(setAppStartInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteStartInfo = getAppRemoteStartInfo;
		SetAppStartInfo = setAppStartInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(UpdateAppContext context, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound)
	{
		Arg.IsValid(context, "context");
		LogInfo("Executing UpdateAppStartInfo...");
		UpdateAppContext updateAppContext = context;
		LogTrace("context = " + updateAppContext.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(context, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppStartInfo finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppStartInfo finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (AppRemoteVersionNotFound e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppStartInfo finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			onAppRemoteVersionNotFound(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("UpdateAppStartInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("UpdateAppStartInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("UpdateAppStartInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public UpdateAppStartInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new UpdateAppStartInfo(context.Resolve<GetAppRemoteStartInfoDelegate>(), context.Resolve<SetAppStartInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
