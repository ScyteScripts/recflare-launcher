using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Network;

public class GetHttpStreamedResponse
{
	private class HttpStreamedResponse : IDisposable, IHttpStreamedResponseDisposable, IHttpStreamedResponse, IBaseDisposable
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "HttpStreamedResponse";

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public HttpStatusCode StatusCode { get; }

		public HttpResponseCharacterSet CharacterSet { get; }

		public IHttpResponseBodyStream BodyStream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		public HttpStreamedResponse(HttpStatusCode statusCode, HttpResponseCharacterSet characterSet, IHttpResponseBodyStream bodyStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(bodyStream, "bodyStream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			StatusCode = statusCode;
			CharacterSet = characterSet;
			BodyStream = bodyStream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~HttpStreamedResponse()
		{
			Dispose(isDisposing: false);
		}

		public void Dispose()
		{
			Dispose(isDisposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool isDisposing)
		{
			if (_disposalHandler.IsDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				DisposeManaged();
				if (_attachedDisposables != null)
				{
					foreach (IDisposable attachedDisposable in _attachedDisposables)
					{
						attachedDisposable.Dispose();
					}
				}
			}
			DisposeUnmanaged();
			_disposalHandler.MarkAsDisposed();
		}

		public void AttachDisposable(IDisposable disposable)
		{
			Assert.IsNotNull(disposable);
			if (_attachedDisposables == null)
			{
				_attachedDisposables = new List<IDisposable>();
			}
			_attachedDisposables.Add(disposable);
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

	private class HttpResponseBodyStream : IDisposable, IHttpResponseBodyStreamDisposable, IHttpResponseBodyStream, IBaseDisposable
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "HttpResponseBodyStream";

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private Stream Stream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
			Stream.Dispose();
		}

		private void DisposeUnmanaged()
		{
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer)
		{
			int v;
			try
			{
				v = Stream.Read(buffer.Array, buffer.Offset, buffer.Length);
			}
			catch (HttpRequestException inner)
			{
				throw new ConnectionFailure(inner);
			}
			catch (OperationCanceledException inner2)
			{
				throw new ConnectionFailure(inner2);
			}
			catch (IOException inner3)
			{
				throw new ConnectionFailure(inner3);
			}
			catch (WebException inner4)
			{
				throw new ConnectionFailure(inner4);
			}
			return new StreamReadResult(new StreamReadLength(v));
		}

		public StreamReadResult Read(Buffer<byte> buffer, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure)
		{
			_disposalHandler.ThrowIfDisposed();
			Arg.IsValid(buffer, "buffer");
			try
			{
				return ExecuteRead(buffer);
			}
			catch (ConnectionFailure e)
			{
				if (onConnectionFailure == null)
				{
					throw;
				}
				object obj = onConnectionFailure(e);
				if (obj == null)
				{
					return default;
				}
				return (StreamReadResult)obj;
			}
			catch (OperationCanceledException)
			{
				LogInfo("HttpResponseBodyStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("HttpResponseBodyStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("HttpResponseBodyStream::Read finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public HttpResponseBodyStream(Stream stream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(stream, "stream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Stream = stream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~HttpResponseBodyStream()
		{
			Dispose(isDisposing: false);
		}

		public void Dispose()
		{
			Dispose(isDisposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool isDisposing)
		{
			if (_disposalHandler.IsDisposed)
			{
				return;
			}
			if (isDisposing)
			{
				DisposeManaged();
				if (_attachedDisposables != null)
				{
					foreach (IDisposable attachedDisposable in _attachedDisposables)
					{
						attachedDisposable.Dispose();
					}
				}
			}
			DisposeUnmanaged();
			_disposalHandler.MarkAsDisposed();
		}

		public void AttachDisposable(IDisposable disposable)
		{
			Assert.IsNotNull(disposable);
			if (_attachedDisposables == null)
			{
				_attachedDisposables = new List<IDisposable>();
			}
			_attachedDisposables.Add(disposable);
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

	private static readonly MethodInfo HttpWebRequestAddWithoutValidate = typeof(WebHeaderCollection).GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IHttpStreamedResponseDisposable Execute(HttpRequestInfo requestInfo, Timeout? timeout)
	{
		HttpWebRequest httpWebRequest = CreateRequest(requestInfo.Address);
		SetRequestMethod(httpWebRequest, requestInfo.Method);
		SetRequestTimeout(httpWebRequest, timeout);
		if (requestInfo.BytesRange.HasValue)
		{
			SetRequestRange(httpWebRequest, requestInfo.BytesRange.Value);
		}
		foreach (HttpRequestHeader header in requestInfo.Headers)
		{
			HttpWebRequestAddWithoutValidate.Invoke(httpWebRequest.Headers, new object[2]
			{
				(string)header.Name,
				(string)header.Value
			});
		}
		if (requestInfo.Content.HasValue)
		{
			SetRequestContent(httpWebRequest, requestInfo.Content.Value);
		}
		HttpWebResponse requestResponse = GetRequestResponse(httpWebRequest);
		string text = requestResponse.CharacterSet;
		if (string.IsNullOrEmpty(text))
		{
			text = "ISO-8859-1";
		}
		Stream responseStream = requestResponse.GetResponseStream();
		Assert.IsNotNull(responseStream);
		HttpResponseBodyStream httpResponseBodyStream = NewHttpResponseBodyStream(responseStream);
		try
		{
			HttpStreamedResponse httpStreamedResponse = NewHttpStreamedResponse(requestResponse.StatusCode, new HttpResponseCharacterSet(text), httpResponseBodyStream);
			httpStreamedResponse.AttachDisposable(httpResponseBodyStream);
			return httpStreamedResponse;
		}
		catch (Exception)
		{
			httpResponseBodyStream.Dispose();
			throw;
		}
	}

	private HttpWebRequest CreateRequest(HttpAddress address)
	{
		HttpWebRequest httpWebRequest = WebRequest.CreateHttp(StudioPatch.ServiceRouting.ResolveUri(address));
		Assert.IsNotNull(httpWebRequest);
		return httpWebRequest;
	}

	private void SetRequestMethod(HttpWebRequest request, HttpRequestMethod method)
	{
		switch (method)
		{
		case HttpRequestMethod.Get:
			request.Method = "Get";
			break;
		case HttpRequestMethod.Post:
			request.Method = "Post";
			break;
		default:
			throw new UnpredictedCondition("Unsupported HTTP request method: " + request.Method);
		}
	}

	private void SetRequestTimeout(HttpWebRequest request, Timeout? timeout)
	{
		if (timeout.HasValue)
		{
			request.Timeout = (int)Math.Min(2147483647.0, timeout.Value.Value.TotalMilliseconds);
		}
		else
		{
			request.Timeout = -1;
		}
	}

	private void SetRequestRange(HttpWebRequest request, HttpRequestBytesRange range)
	{
		string text = range.Start.ToString();
		string text2 = (range.End.HasValue ? range.End.ToString() : string.Empty);
		Assert.IsNotNull(HttpWebRequestAddWithoutValidate);
		HttpWebRequestAddWithoutValidate.Invoke(request.Headers, new object[2]
		{
			"Range",
			"bytes=" + text + "-" + text2
		});
	}

	private void SetRequestContent(HttpWebRequest request, HttpRequestContent content)
	{
		switch (content.Type)
		{
		case HttpRequestContentType.ApplicationXWWWFormUrlEncoded:
			request.ContentType = "application/x-www-form-urlencoded";
			break;
		case HttpRequestContentType.MultipartFormData:
			request.ContentType = "multipart/form-data";
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		request.ContentLength = content.Stream.Length;
		Stream requestStream;
		try
		{
			requestStream = request.GetRequestStream();
		}
		catch (HttpRequestException inner)
		{
			throw new ConnectionFailure(inner);
		}
		catch (OperationCanceledException inner2)
		{
			throw new ConnectionFailure(inner2);
		}
		catch (WebException inner3)
		{
			throw new ConnectionFailure(inner3);
		}
		Buffer<byte> buffer = Buffer<byte>.CreateOfLength(new BufferLength(4194304));
		while (true)
		{
			StreamReadResult streamReadResult2;
			StreamReadResult streamReadResult = (streamReadResult2 = content.Stream.Read(buffer));
			if (streamReadResult.IsNotEndOfStream)
			{
				requestStream.Write(buffer.Array, 0, streamReadResult2.Length);
				continue;
			}
			break;
		}
	}

	private HttpWebResponse GetRequestResponse(HttpWebRequest request)
	{
		try
		{
			return (HttpWebResponse)request.GetResponse();
		}
		catch (HttpRequestException inner)
		{
			throw new ConnectionFailure(inner);
		}
		catch (OperationCanceledException inner2)
		{
			throw new ConnectionFailure(inner2);
		}
		catch (WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
			{
				return (HttpWebResponse)ex.Response;
			}
			throw new ConnectionFailure(ex);
		}
	}

	private HttpStreamedResponse NewHttpStreamedResponse(HttpStatusCode statusCode, HttpResponseCharacterSet characterSet, IHttpResponseBodyStream bodyStream)
	{
		return new HttpStreamedResponse(statusCode, characterSet, bodyStream, Log, BeginLogRegion, EndLogRegion);
	}

	private HttpResponseBodyStream NewHttpResponseBodyStream(Stream stream)
	{
		return new HttpResponseBodyStream(stream, Log, BeginLogRegion, EndLogRegion);
	}

	public GetHttpStreamedResponse(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IHttpStreamedResponseDisposable DelegateExecute(HttpRequestInfo requestInfo, Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure)
	{
		Arg.IsValid(requestInfo, "requestInfo");
		Arg.IsValid(timeout, "timeout");
		LogInfo("Executing GetHttpStreamedResponse...");
		HttpRequestInfo httpRequestInfo = requestInfo;
		LogTrace("requestInfo = " + httpRequestInfo.ToString());
		Timeout? timeout2 = timeout;
		LogTrace("timeout = " + timeout2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IHttpStreamedResponseDisposable httpStreamedResponseDisposable = Execute(requestInfo, timeout);
			EndLogRegion(enabled: true);
			LogTrace("result = " + httpStreamedResponseDisposable);
			LogInfo("GetHttpStreamedResponse finished: success.");
			return httpStreamedResponseDisposable;
		}
		catch (ConnectionFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetHttpStreamedResponse finished: ConnectionFailure.");
			if (onConnectionFailure == null)
			{
				throw;
			}
			object obj = onConnectionFailure(e);
			if (obj == null)
			{
				return null;
			}
			return (IHttpStreamedResponseDisposable)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetHttpStreamedResponse has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetHttpStreamedResponse failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetHttpStreamedResponse finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetHttpStreamedResponseDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetHttpStreamedResponse(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
