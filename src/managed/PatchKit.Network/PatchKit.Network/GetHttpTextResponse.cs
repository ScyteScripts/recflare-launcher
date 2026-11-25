using System;
using System.Linq;
using System.Text;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public class GetHttpTextResponse
{
	public GetHttpResponseDelegate GetHttpResponse { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private HttpTextResponse Execute(HttpRequestInfo requestInfo, Timeout? timeout)
	{
		HttpResponse httpResponse = GetHttpResponse(requestInfo, timeout, null);
		Encoding encoding = Encoding.GetEncoding(httpResponse.CharacterSet);
		return new HttpTextResponse(body: new Text(encoding.GetString(httpResponse.Body.ToArray())), statusCode: httpResponse.StatusCode);
	}

	public GetHttpTextResponse(GetHttpResponseDelegate getHttpResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getHttpResponse);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetHttpResponse = getHttpResponse;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private HttpTextResponse DelegateExecute(HttpRequestInfo requestInfo, Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure)
	{
		Arg.IsValid(requestInfo, "requestInfo");
		Arg.IsValid(timeout, "timeout");
		LogInfo("Executing GetHttpTextResponse...");
		HttpRequestInfo httpRequestInfo = requestInfo;
		LogTrace("requestInfo = " + httpRequestInfo.ToString());
		Timeout? timeout2 = timeout;
		LogTrace("timeout = " + timeout2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			HttpTextResponse httpTextResponse = Execute(requestInfo, timeout);
			EndLogRegion(enabled: true);
			HttpTextResponse httpTextResponse2 = httpTextResponse;
			LogTrace("result = " + httpTextResponse2.ToString());
			LogInfo("GetHttpTextResponse finished: success.");
			return httpTextResponse;
		}
		catch (ConnectionFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetHttpTextResponse finished: ConnectionFailure.");
			if (onConnectionFailure == null)
			{
				throw;
			}
			object obj = onConnectionFailure(e);
			if (obj == null)
			{
				return default;
			}
			return (HttpTextResponse)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetHttpTextResponse has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetHttpTextResponse failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetHttpTextResponse finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetHttpTextResponseDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetHttpTextResponse(context.Resolve<GetHttpResponseDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
