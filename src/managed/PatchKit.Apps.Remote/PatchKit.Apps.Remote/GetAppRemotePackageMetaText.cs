using System;
using System.Net;
using System.Threading;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public class GetAppRemotePackageMetaText
{
	public GetHttpTextResponseDelegate GetHttpTextResponse { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Text Execute(ImmutableArray<HttpAddress> urls, Text? authHeaderValue, CancellationToken cancellationToken)
	{
		Text? text = null;
		int num = 0;
		while (!text.HasValue)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				ImmutableArray<PatchKit.Network.HttpRequestHeader> headers = ImmutableArray<PatchKit.Network.HttpRequestHeader>.Empty;
				if (authHeaderValue.HasValue)
				{
					headers = new PatchKit.Network.HttpRequestHeader[1]
					{
						new PatchKit.Network.HttpRequestHeader(new Text("Authorization"), authHeaderValue.Value)
					}.AsImmutable();
				}
				HttpTextResponse httpTextResponse = GetHttpTextResponse(new HttpRequestInfo(urls[num], HttpRequestMethod.Get, null, null, headers), new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), null);
				if (httpTextResponse.StatusCode == HttpStatusCode.Unauthorized)
				{
					throw new InvalidAppAuthKey();
				}
				if (httpTextResponse.StatusCode == HttpStatusCode.Gone)
				{
					throw new ExpiredAppAuthKey();
				}
				if (httpTextResponse.StatusCode >= HttpStatusCode.OK && httpTextResponse.StatusCode <= (HttpStatusCode)299)
				{
					text = httpTextResponse.Body;
				}
			}
			catch (ConnectionFailure exception)
			{
				LogWarning("Failed to retrieve package meta text. Retrying...", exception);
				num++;
				if (num >= urls.Length)
				{
					num = 0;
				}
			}
		}
		return text.Value;
	}

	public GetAppRemotePackageMetaText(GetHttpTextResponseDelegate getHttpTextResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getHttpTextResponse);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetHttpTextResponse = getHttpTextResponse;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Text DelegateExecute(ImmutableArray<HttpAddress> urls, Text? authHeaderValue, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsValid(urls, "urls");
		Arg.IsValid(authHeaderValue, "authHeaderValue");
		LogInfo("Executing GetAppRemotePackageMetaText...");
		ImmutableArray<HttpAddress> immutableArray = urls;
		LogTrace("urls = " + immutableArray.ToString());
		Text? text = authHeaderValue;
		LogTrace("authHeaderValue = " + (text.HasValue ? ((string)text.GetValueOrDefault()) : null));
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			Text text2 = Execute(urls, authHeaderValue, cancellationToken);
			EndLogRegion(enabled: false);
			LogTrace("result = " + text2);
			LogInfo("GetAppRemotePackageMetaText finished: success.");
			return text2;
		}
		catch (InvalidAppAuthKey e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePackageMetaText finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj = onInvalidAppAuthKey(e);
			if (obj == null)
			{
				return default;
			}
			return (Text)obj;
		}
		catch (ExpiredAppAuthKey e2)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePackageMetaText finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj2 = onExpiredAppAuthKey(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (Text)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePackageMetaText has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePackageMetaText failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePackageMetaText finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemotePackageMetaTextDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemotePackageMetaText(context.Resolve<GetHttpTextResponseDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
