using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppEncryptedPassword
{
	public GetAppInfoDelegate GetAppInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppEncryptedPassword? Execute(App app)
	{
		return GetAppInfo(app, null).EncryptedPassword;
	}

	public GetAppEncryptedPassword(GetAppInfoDelegate getAppInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppInfo = getAppInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppEncryptedPassword? DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GetAppEncryptedPassword...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			AppEncryptedPassword? appEncryptedPassword = Execute(app);
			EndLogRegion(enabled: true);
			AppEncryptedPassword? appEncryptedPassword2 = appEncryptedPassword;
			LogTrace("result = " + (appEncryptedPassword2.HasValue ? ((string)appEncryptedPassword2.GetValueOrDefault()) : null));
			LogInfo("GetAppEncryptedPassword finished: success.");
			return appEncryptedPassword;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppEncryptedPassword finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppEncryptedPassword?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppEncryptedPassword has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppEncryptedPassword failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppEncryptedPassword finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppEncryptedPasswordDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppEncryptedPassword(context.Resolve<GetAppInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
