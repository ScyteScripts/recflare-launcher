using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class IsAppRemoteLicenseSecretRequired
{
	public GetAppRemoteInfoDelegate GetAppRemoteInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(AppRemote appRemote, CancellationToken cancellationToken)
	{
		return GetAppRemoteInfo(appRemote, cancellationToken, null).IsLicenseSecretRequired;
	}

	public IsAppRemoteLicenseSecretRequired(GetAppRemoteInfoDelegate getAppRemoteInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteInfo = getAppRemoteInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(AppRemote appRemote, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound)
	{
		Arg.IsNotNull(appRemote, "appRemote");
		LogInfo("Executing IsAppRemoteLicenseSecretRequired...");
		LogTrace("appRemote = " + appRemote);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(appRemote, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsAppRemoteLicenseSecretRequired finished: success.");
			return result;
		}
		catch (AppRemoteNotFound e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppRemoteLicenseSecretRequired finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj = onAppRemoteNotFound(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsAppRemoteLicenseSecretRequired has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsAppRemoteLicenseSecretRequired failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsAppRemoteLicenseSecretRequired finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsAppRemoteLicenseSecretRequiredDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsAppRemoteLicenseSecretRequired(context.Resolve<GetAppRemoteInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
