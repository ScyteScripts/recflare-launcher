using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Network;

public class GetBufferedHttpStreamedResponse
{
	private class BufferedHttpStreamedResponse : IDisposable, IBufferedHttpStreamedResponseDisposable, IBufferedHttpStreamedResponse, IBaseDisposable
	{
		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "BufferedHttpStreamedResponse";

		private GetHttpStreamedResponseDelegate GetHttpStreamedResponse { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public HttpStatusCode StatusCode { get; }

		public HttpResponseCharacterSet CharacterSet { get; }

		public IBufferedHttpResponseBodyStream BodyStream { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		public BufferedHttpStreamedResponse(HttpStatusCode statusCode, HttpResponseCharacterSet characterSet, IBufferedHttpResponseBodyStream bodyStream, GetHttpStreamedResponseDelegate getHttpStreamedResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(bodyStream, "bodyStream");
			Arg.IsNotNull(getHttpStreamedResponse, "getHttpStreamedResponse");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			StatusCode = statusCode;
			CharacterSet = characterSet;
			BodyStream = bodyStream;
			GetHttpStreamedResponse = getHttpStreamedResponse;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~BufferedHttpStreamedResponse()
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

	private class BufferedHttpResponseBodyStream : IDisposable, IBufferedHttpResponseBodyStreamDisposable, IHttpResponseBodyStreamDisposable, IHttpResponseBodyStream, IBaseDisposable, IBufferedHttpResponseBodyStream
	{
		private bool _hasStreamEnded = false;

		private bool _shouldFillCircleBuffer = true;

		public const int ReadBufferCapacity = 8388608;

		private byte[] _readBuffer = new byte[8388608];

		public const int CircleBufferCapacity = 134217728;

		public const int CircleBufferWriteLeftBytesThreshold = 67108864;

		public const int WaitForCircleBufferLeftBytesDurationMs = 100;

		public const int WaitForCircleBufferWrittenBytesDurationMs = 100;

		private byte[] _circleBuffer = new byte[134217728];

		private int _circleBufferStartIndex = 0;

		private int _circleBufferEndIndex = 0;

		private object _circleBufferLock = new object();

		private Exception _fillCircleBufferThreadException = null;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "BufferedHttpResponseBodyStream";

		private int NonThreadSafeCircleBufferLength
		{
			get
			{
				if (_circleBufferEndIndex > _circleBufferStartIndex)
				{
					return _circleBufferEndIndex - _circleBufferStartIndex;
				}
				if (_circleBufferEndIndex < _circleBufferStartIndex)
				{
					return _circleBuffer.Length - _circleBufferStartIndex + _circleBufferEndIndex;
				}
				return 0;
			}
		}

		private int NonThreadSafeCircleBufferLeftBytes => _circleBuffer.Length - NonThreadSafeCircleBufferLength - 1;

		private int CircleBufferLength
		{
			get
			{
				lock (_circleBufferLock)
				{
					return NonThreadSafeCircleBufferLength;
				}
			}
		}

		private int CircleBufferLeftBytes
		{
			get
			{
				lock (_circleBufferLock)
				{
					return NonThreadSafeCircleBufferLeftBytes;
				}
			}
		}

		private GetHttpStreamedResponseDelegate GetHttpStreamedResponse { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private IHttpResponseBodyStream SourceStream { get; }

		private void WriteToCircleBuffer(byte[] array, int length)
		{
			lock (_circleBufferLock)
			{
				bool v = NonThreadSafeCircleBufferLeftBytes >= length;
				Assert.IsTrue(v, "circleBufferHaveRequiredAmountOfLeftBytes");
				if (_circleBufferEndIndex + length >= _circleBuffer.Length)
				{
					int num = _circleBuffer.Length - _circleBufferEndIndex;
					int num2 = length - num;
					Array.Copy(array, 0, _circleBuffer, _circleBufferEndIndex, num);
					Array.Copy(array, num, _circleBuffer, 0, num2);
					_circleBufferEndIndex = num2;
				}
				else
				{
					Array.Copy(array, 0, _circleBuffer, _circleBufferEndIndex, length);
					_circleBufferEndIndex += length;
				}
			}
		}

		private void ReadFromCircleBuffer(byte[] array, int offset, int length)
		{
			lock (_circleBufferLock)
			{
				bool v = NonThreadSafeCircleBufferLength >= length;
				Assert.IsTrue(v, "circleBufferHaveRequiredAmountOfWrittenBytes");
				if (_circleBufferStartIndex + length < _circleBuffer.Length)
				{
					Array.Copy(_circleBuffer, _circleBufferStartIndex, array, offset, length);
					_circleBufferStartIndex += length;
					return;
				}
				int num = _circleBuffer.Length - _circleBufferStartIndex;
				int num2 = length - num;
				Array.Copy(_circleBuffer, _circleBufferStartIndex, array, offset, num);
				Array.Copy(_circleBuffer, 0, array, offset + num, num2);
				_circleBufferStartIndex = num2;
			}
		}

		private void FillCircleBufferThread()
		{
			try
			{
				while (!_hasStreamEnded && !_disposalHandler.IsDisposed)
				{
					int circleBufferLeftBytes = CircleBufferLeftBytes;
					if (circleBufferLeftBytes >= 8388608 && _shouldFillCircleBuffer)
					{
						if (_disposalHandler.IsDisposed)
						{
							break;
						}
						StreamReadResult streamReadResult;
						try
						{
							streamReadResult = SourceStream.Read(new Buffer<byte>(_readBuffer, new BufferOffset(0), new BufferLength(8388608)), (ConnectionFailure e) => (object)null);
						}
						catch (UsageOfDisposedObject)
						{
							break;
						}
						if (streamReadResult.IsEndOfStream)
						{
							_hasStreamEnded = true;
						}
						else
						{
							WriteToCircleBuffer(_readBuffer, streamReadResult.Length);
						}
					}
					else
					{
						_shouldFillCircleBuffer = circleBufferLeftBytes > 67108864;
						Thread.Sleep(100);
					}
				}
			}
			catch (Exception fillCircleBufferThreadException)
			{
				_fillCircleBufferThreadException = fillCircleBufferThreadException;
			}
		}

		private void Constructor()
		{
			ThreadPool.QueueUserWorkItem((object _) =>
			{
				FillCircleBufferThread();
			});
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		private int WaitForCircleBufferWrittenBytes()
		{
			int circleBufferLength = CircleBufferLength;
			bool hasStreamEnded;
			do
			{
				if (_fillCircleBufferThreadException != null)
				{
					throw _fillCircleBufferThreadException;
				}
				hasStreamEnded = _hasStreamEnded;
				circleBufferLength = CircleBufferLength;
				if (circleBufferLength == 0 && !hasStreamEnded)
				{
					Thread.Sleep(100);
				}
			}
			while (circleBufferLength == 0 && !hasStreamEnded);
			return circleBufferLength;
		}

		private BytesCount ExecuteGetBufferedBytes()
		{
			return new BytesCount(CircleBufferLength);
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer)
		{
			int val = WaitForCircleBufferWrittenBytes();
			int num = Math.Min(val, buffer.Length);
			ReadFromCircleBuffer(buffer.Array, buffer.Offset, num);
			return new StreamReadResult(new StreamReadLength(num));
		}

		public BytesCount GetBufferedBytes()
		{
			_disposalHandler.ThrowIfDisposed();
			try
			{
				return ExecuteGetBufferedBytes();
			}
			catch (OperationCanceledException)
			{
				LogInfo("BufferedHttpResponseBodyStream::GetBufferedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("BufferedHttpResponseBodyStream::GetBufferedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("BufferedHttpResponseBodyStream::GetBufferedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
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
				LogInfo("BufferedHttpResponseBodyStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("BufferedHttpResponseBodyStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("BufferedHttpResponseBodyStream::Read finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public BufferedHttpResponseBodyStream(IHttpResponseBodyStream sourceStream, GetHttpStreamedResponseDelegate getHttpStreamedResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(sourceStream, "sourceStream");
			Arg.IsNotNull(getHttpStreamedResponse, "getHttpStreamedResponse");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			SourceStream = sourceStream;
			GetHttpStreamedResponse = getHttpStreamedResponse;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~BufferedHttpResponseBodyStream()
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

	public GetHttpStreamedResponseDelegate GetHttpStreamedResponse { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IBufferedHttpStreamedResponseDisposable Execute(HttpRequestInfo requestInfo, PatchKit.Core.Timeout? timeout)
	{
		IHttpStreamedResponseDisposable httpStreamedResponseDisposable = GetHttpStreamedResponse(requestInfo, timeout, null);
		Assert.IsNotNullFromDelegate(httpStreamedResponseDisposable);
		try
		{
			BufferedHttpStreamedResponse bufferedHttpStreamedResponse = NewBufferedHttpStreamedResponse(httpStreamedResponseDisposable.StatusCode, httpStreamedResponseDisposable.CharacterSet, NewBufferedHttpResponseBodyStream(httpStreamedResponseDisposable.BodyStream));
			bufferedHttpStreamedResponse.AttachDisposable(httpStreamedResponseDisposable);
			return bufferedHttpStreamedResponse;
		}
		catch
		{
			httpStreamedResponseDisposable.Dispose();
			throw;
		}
	}

	private BufferedHttpStreamedResponse NewBufferedHttpStreamedResponse(HttpStatusCode statusCode, HttpResponseCharacterSet characterSet, IBufferedHttpResponseBodyStream bodyStream)
	{
		return new BufferedHttpStreamedResponse(statusCode, characterSet, bodyStream, GetHttpStreamedResponse, Log, BeginLogRegion, EndLogRegion);
	}

	private BufferedHttpResponseBodyStream NewBufferedHttpResponseBodyStream(IHttpResponseBodyStream sourceStream)
	{
		return new BufferedHttpResponseBodyStream(sourceStream, GetHttpStreamedResponse, Log, BeginLogRegion, EndLogRegion);
	}

	public GetBufferedHttpStreamedResponse(GetHttpStreamedResponseDelegate getHttpStreamedResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

	private IBufferedHttpStreamedResponseDisposable DelegateExecute(HttpRequestInfo requestInfo, PatchKit.Core.Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure)
	{
		Arg.IsValid(requestInfo, "requestInfo");
		Arg.IsValid(timeout, "timeout");
		LogInfo("Executing GetBufferedHttpStreamedResponse...");
		HttpRequestInfo httpRequestInfo = requestInfo;
		LogTrace("requestInfo = " + httpRequestInfo.ToString());
		PatchKit.Core.Timeout? timeout2 = timeout;
		LogTrace("timeout = " + timeout2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IBufferedHttpStreamedResponseDisposable bufferedHttpStreamedResponseDisposable = Execute(requestInfo, timeout);
			EndLogRegion(enabled: true);
			LogTrace("result = " + bufferedHttpStreamedResponseDisposable);
			LogInfo("GetBufferedHttpStreamedResponse finished: success.");
			return bufferedHttpStreamedResponseDisposable;
		}
		catch (ConnectionFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetBufferedHttpStreamedResponse finished: ConnectionFailure.");
			if (onConnectionFailure == null)
			{
				throw;
			}
			object obj = onConnectionFailure(e);
			if (obj == null)
			{
				return null;
			}
			return (IBufferedHttpStreamedResponseDisposable)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetBufferedHttpStreamedResponse has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetBufferedHttpStreamedResponse failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetBufferedHttpStreamedResponse finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetBufferedHttpStreamedResponseDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetBufferedHttpStreamedResponse(context.Resolve<GetHttpStreamedResponseDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
