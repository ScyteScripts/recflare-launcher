using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class SetAppPassword
{
	public EncryptAppPasswordDelegate EncryptAppPassword { get; }

	public GetAppInfoDelegate GetAppInfo { get; }

	public SetAppInfoDelegate SetAppInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPassword appPassword, AppSecret appSecret)
	{
		AppEncryptedPassword value = EncryptAppPassword(appPassword, appSecret);
		AppInfo value2 = GetAppInfo(app, null).ModifyEncryptedPassword(value);
		SetAppInfo(app, value2, null);
	}

	public SetAppPassword(EncryptAppPasswordDelegate encryptAppPassword, GetAppInfoDelegate getAppInfo, SetAppInfoDelegate setAppInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(encryptAppPassword);
		Assert.IsNotNull(getAppInfo);
		Assert.IsNotNull(setAppInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		EncryptAppPassword = encryptAppPassword;
		GetAppInfo = getAppInfo;
		SetAppInfo = setAppInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPassword appPassword, AppSecret appSecret, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(appPassword, "appPassword");
		Arg.IsValid(appSecret, "appSecret");
		LogInfo("Executing SetAppPassword...");
		LogTrace("app = " + app);
		LogTrace("appPassword = " + appPassword);
		LogTrace("appSecret = " + appSecret);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, appPassword, appSecret);
			EndLogRegion(enabled: true);
			LogInfo("SetAppPassword finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("SetAppPassword finished: UnauthorizedAccess.");
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
			LogInfo("SetAppPassword has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppPassword failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SetAppPassword finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SetAppPasswordDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SetAppPassword(context.Resolve<EncryptAppPasswordDelegate>(), context.Resolve<GetAppInfoDelegate>(), context.Resolve<SetAppInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
