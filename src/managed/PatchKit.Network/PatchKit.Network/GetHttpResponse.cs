using System;
using System.Collections.Generic;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Network;

public class GetHttpResponse
{
	public GetHttpStreamedResponseDelegate GetHttpStreamedResponse { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private HttpResponse Execute(HttpRequestInfo requestInfo, Timeout? timeout)
	{
		using IHttpStreamedResponseDisposable httpStreamedResponseDisposable = GetHttpStreamedResponse(requestInfo, timeout, null);
		Assert.IsNotNullFromDelegate(httpStreamedResponseDisposable);
		List<byte> list = new List<byte>();
		Buffer<byte> buffer = Buffer<byte>.CreateOfLength(new BufferLength(4194304));
		while (true)
		{
			StreamReadResult streamReadResult2;
			StreamReadResult streamReadResult = (streamReadResult2 = httpStreamedResponseDisposable.BodyStream.Read(buffer, null));
			if (!streamReadResult.IsNotEndOfStream)
			{
				break;
			}
			for (int i = 0; i < (int)streamReadResult2.Length; i++)
			{
				list.Add(buffer.Array[i]);
			}
		}
		return new HttpResponse(httpStreamedResponseDisposable.StatusCode, httpStreamedResponseDisposable.CharacterSet, list.AsImmutable());
	}

	public GetHttpResponse(GetHttpStreamedResponseDelegate getHttpStreamedResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getHttpStreamedResponse);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetHttpStreamedResponse = getHttpStreamedResponse;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private HttpResponse DelegateExecute(HttpRequestInfo requestInfo, Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure)
	{
		Arg.IsValid(requestInfo, "requestInfo");
		Arg.IsValid(timeout, "timeout");
		LogInfo("Executing GetHttpResponse...");
		HttpRequestInfo httpRequestInfo = requestInfo;
		LogTrace("requestInfo = " + httpRequestInfo.ToString());
		Timeout? timeout2 = timeout;
		LogTrace("timeout = " + timeout2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			HttpResponse httpResponse = Execute(requestInfo, timeout);
			EndLogRegion(enabled: true);
			HttpResponse httpResponse2 = httpResponse;
			LogTrace("result = " + httpResponse2.ToString());
			LogInfo("GetHttpResponse finished: success.");
			return httpResponse;
		}
		catch (ConnectionFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetHttpResponse finished: ConnectionFailure.");
			if (onConnectionFailure == null)
			{
				throw;
			}
			object obj = onConnectionFailure(e);
			if (obj == null)
			{
				return default;
			}
			return (HttpResponse)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetHttpResponse has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetHttpResponse failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetHttpResponse finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetHttpResponseDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetHttpResponse(context.Resolve<GetHttpStreamedResponseDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
