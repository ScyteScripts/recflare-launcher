using System;
using System.Threading;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public class GetAppRemotePack1UnpackInfoB
{
	public GetAppRemotePack1UnpackInfoADelegate GetAppRemotePack1UnpackInfoA { get; }

	public GetAppRemotePackageMetaTextDelegate GetAppRemotePackageMetaText { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePack1UnpackInfo Execute(ImmutableArray<HttpAddress> metaUrls, AppRemotePack1UnpackPassword password, Text? authHeaderValue, CancellationToken cancellationToken)
	{
		Text metaText = GetAppRemotePackageMetaText(metaUrls, authHeaderValue, cancellationToken, null, null);
		return GetAppRemotePack1UnpackInfoA(metaText, password);
	}

	public GetAppRemotePack1UnpackInfoB(GetAppRemotePack1UnpackInfoADelegate getAppRemotePack1UnpackInfoA, GetAppRemotePackageMetaTextDelegate getAppRemotePackageMetaText, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemotePack1UnpackInfoA);
		Assert.IsNotNull(getAppRemotePackageMetaText);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemotePack1UnpackInfoA = getAppRemotePack1UnpackInfoA;
		GetAppRemotePackageMetaText = getAppRemotePackageMetaText;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePack1UnpackInfo DelegateExecute(ImmutableArray<HttpAddress> metaUrls, AppRemotePack1UnpackPassword password, Text? authHeaderValue, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsValid(metaUrls, "metaUrls");
		Arg.IsValid(password, "password");
		Arg.IsValid(authHeaderValue, "authHeaderValue");
		LogInfo("Executing GetAppRemotePack1UnpackInfoB...");
		ImmutableArray<HttpAddress> immutableArray = metaUrls;
		LogTrace("metaUrls = " + immutableArray.ToString());
		LogTrace("password = " + password);
		Text? text = authHeaderValue;
		LogTrace("authHeaderValue = " + (text.HasValue ? ((string)text.GetValueOrDefault()) : null));
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemotePack1UnpackInfo appRemotePack1UnpackInfo = Execute(metaUrls, password, authHeaderValue, cancellationToken);
			EndLogRegion(enabled: false);
			AppRemotePack1UnpackInfo appRemotePack1UnpackInfo2 = appRemotePack1UnpackInfo;
			LogTrace("result = " + appRemotePack1UnpackInfo2.ToString());
			LogInfo("GetAppRemotePack1UnpackInfoB finished: success.");
			return appRemotePack1UnpackInfo;
		}
		catch (InvalidAppAuthKey e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePack1UnpackInfoB finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj = onInvalidAppAuthKey(e);
			if (obj == null)
			{
				return default;
			}
			return (AppRemotePack1UnpackInfo)obj;
		}
		catch (ExpiredAppAuthKey e2)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePack1UnpackInfoB finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj2 = onExpiredAppAuthKey(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (AppRemotePack1UnpackInfo)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePack1UnpackInfoB has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePack1UnpackInfoB failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePack1UnpackInfoB finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemotePack1UnpackInfoBDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemotePack1UnpackInfoB(context.Resolve<GetAppRemotePack1UnpackInfoADelegate>(), context.Resolve<GetAppRemotePackageMetaTextDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
