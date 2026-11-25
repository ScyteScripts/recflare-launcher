using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppPassword
{
	public GetAppEncryptedPasswordDelegate GetAppEncryptedPassword { get; }

	public DecryptAppEncryptedPasswordDelegate DecryptAppEncryptedPassword { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppPassword? Execute(App app, AppSecret appSecret)
	{
		AppEncryptedPassword? appEncryptedPassword = GetAppEncryptedPassword(app, null);
		if (!appEncryptedPassword.HasValue)
		{
			return null;
		}
		return DecryptAppEncryptedPassword(appEncryptedPassword.Value, appSecret);
	}

	public GetAppPassword(GetAppEncryptedPasswordDelegate getAppEncryptedPassword, DecryptAppEncryptedPasswordDelegate decryptAppEncryptedPassword, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEncryptedPassword);
		Assert.IsNotNull(decryptAppEncryptedPassword);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEncryptedPassword = getAppEncryptedPassword;
		DecryptAppEncryptedPassword = decryptAppEncryptedPassword;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppPassword? DelegateExecute(App app, AppSecret appSecret, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(appSecret, "appSecret");
		LogInfo("Executing GetAppPassword...");
		LogTrace("app = " + app);
		LogTrace("appSecret = " + appSecret);
		try
		{
			BeginLogRegion(enabled: true);
			AppPassword? appPassword = Execute(app, appSecret);
			EndLogRegion(enabled: true);
			AppPassword? appPassword2 = appPassword;
			LogTrace("result = " + (appPassword2.HasValue ? ((string)appPassword2.GetValueOrDefault()) : null));
			LogInfo("GetAppPassword finished: success.");
			return appPassword;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppPassword finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppPassword?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppPassword has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppPassword failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppPassword finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppPasswordDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppPassword(context.Resolve<GetAppEncryptedPasswordDelegate>(), context.Resolve<DecryptAppEncryptedPasswordDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
