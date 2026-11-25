using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackageSourceStream
{
	private class PackageSourceStream : IDisposable, IPackageSourceStreamDisposable, IPackageSourceStream, IBaseDisposable
	{
		private IPackageSourcePartStreamDisposable _currentStream;

		private long _currentPosition;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "PackageSourceStream";

		private GetPackageSourcePartStreamDelegate GetPackageSourcePartStream { get; }

		private GetPackageSourcePartStreamRequestDelegate GetPackageSourcePartStreamRequest { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private PackageSourceStreamRequest Request { get; }

		private void Constructor()
		{
			_currentPosition = Request.Offset;
		}

		private void DisposeManaged()
		{
			_currentStream?.Dispose();
		}

		private void DisposeUnmanaged()
		{
		}

		private BytesCount ExecuteGetBufferedBytes()
		{
			if (_currentStream == null)
			{
				return new BytesCount(0L);
			}
			return _currentStream.GetBufferedBytes();
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer, CancellationToken cancellationToken)
		{
			if (_currentStream == null)
			{
				PackageSourcePartStreamRequest? packageSourcePartStreamRequest = GetPackageSourcePartStreamRequest(new PackageSourceStreamContext(Request.Info, new BytesCount(_currentPosition)));
				if (!packageSourcePartStreamRequest.HasValue)
				{
					return StreamReadResult.EndOfStream;
				}
				_currentStream = GetPackageSourcePartStream(packageSourcePartStreamRequest.Value);
				Assert.IsNotNullFromDelegate(_currentStream);
			}
			StreamReadResult result = _currentStream.Read(buffer, cancellationToken, null, null, null);
			if (result.IsEndOfStream)
			{
				_currentStream.Dispose();
				_currentStream = null;
				return ExecuteRead(buffer, cancellationToken);
			}
			_currentPosition += (int)result.Length;
			return result;
		}

		public StreamReadResult Read(Buffer<byte> buffer, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<PackageSourceStreamReadFailure> onPackageSourceStreamReadFailure, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
		{
			_disposalHandler.ThrowIfDisposed();
			Arg.IsValid(buffer, "buffer");
			try
			{
				return ExecuteRead(buffer, cancellationToken);
			}
			catch (PackageSourceStreamReadFailure e)
			{
				if (onPackageSourceStreamReadFailure == null)
				{
					throw;
				}
				object obj = onPackageSourceStreamReadFailure(e);
				if (obj == null)
				{
					return default;
				}
				return (StreamReadResult)obj;
			}
			catch (InvalidAppAuthKey e2)
			{
				if (onInvalidAppAuthKey == null)
				{
					throw;
				}
				object obj2 = onInvalidAppAuthKey(e2);
				if (obj2 == null)
				{
					return default;
				}
				return (StreamReadResult)obj2;
			}
			catch (ExpiredAppAuthKey e3)
			{
				if (onExpiredAppAuthKey == null)
				{
					throw;
				}
				object obj3 = onExpiredAppAuthKey(e3);
				if (obj3 == null)
				{
					return default;
				}
				return (StreamReadResult)obj3;
			}
			catch (OperationCanceledException)
			{
				LogInfo("PackageSourceStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageSourceStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageSourceStream::Read finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
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
				LogInfo("PackageSourceStream::GetBufferedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageSourceStream::GetBufferedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageSourceStream::GetBufferedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public PackageSourceStream(PackageSourceStreamRequest request, GetPackageSourcePartStreamDelegate getPackageSourcePartStream, GetPackageSourcePartStreamRequestDelegate getPackageSourcePartStreamRequest, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(request, "request");
			Arg.IsNotNull(getPackageSourcePartStream, "getPackageSourcePartStream");
			Arg.IsNotNull(getPackageSourcePartStreamRequest, "getPackageSourcePartStreamRequest");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Request = request;
			GetPackageSourcePartStream = getPackageSourcePartStream;
			GetPackageSourcePartStreamRequest = getPackageSourcePartStreamRequest;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~PackageSourceStream()
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

	public GetPackageSourcePartStreamDelegate GetPackageSourcePartStream { get; }

	public GetPackageSourcePartStreamRequestDelegate GetPackageSourcePartStreamRequest { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageSourceStreamDisposable Execute(PackageSourceStreamRequest request)
	{
		return NewPackageSourceStream(request);
	}

	private PackageSourceStream NewPackageSourceStream(PackageSourceStreamRequest request)
	{
		return new PackageSourceStream(request, GetPackageSourcePartStream, GetPackageSourcePartStreamRequest, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPackageSourceStream(GetPackageSourcePartStreamDelegate getPackageSourcePartStream, GetPackageSourcePartStreamRequestDelegate getPackageSourcePartStreamRequest, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getPackageSourcePartStream);
		Assert.IsNotNull(getPackageSourcePartStreamRequest);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetPackageSourcePartStream = getPackageSourcePartStream;
		GetPackageSourcePartStreamRequest = getPackageSourcePartStreamRequest;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageSourceStreamDisposable DelegateExecute(PackageSourceStreamRequest request)
	{
		Arg.IsValid(request, "request");
		LogInfo("Executing GetPackageSourceStream...");
		PackageSourceStreamRequest packageSourceStreamRequest = request;
		LogTrace("request = " + packageSourceStreamRequest.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageSourceStreamDisposable packageSourceStreamDisposable = Execute(request);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageSourceStreamDisposable);
			LogInfo("GetPackageSourceStream finished: success.");
			return packageSourceStreamDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackageSourceStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageSourceStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageSourceStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageSourceStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackageSourceStream(context.Resolve<GetPackageSourcePartStreamDelegate>(), context.Resolve<GetPackageSourcePartStreamRequestDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
