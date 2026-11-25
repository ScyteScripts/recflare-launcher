using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackageMultisourceStream
{
	private class PackageMultisourceStream : IDisposable, IPackageMultisourceStreamDisposable, IPackageMultisourceStream, IBaseDisposable
	{
		private IPackageSourceStreamDisposable _currentStream;

		private long _currentPosition;

		private int _currentSourceIndex = -1;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "PackageMultisourceStream";

		private GetPackageSourceStreamDelegate GetPackageSourceStream { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private PackageMultisourceStreamRequest Request { get; }

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
			StreamReadResult? streamReadResult = null;
			do
			{
				cancellationToken.ThrowIfCancellationRequested();
				try
				{
					if (_currentStream == null)
					{
						_currentStream = GetNextSourceStream();
					}
					streamReadResult = _currentStream.Read(buffer, cancellationToken, null, null, null);
					_currentPosition += (int)streamReadResult.Value.Length;
				}
				catch (PackageSourceStreamReadFailure exception)
				{
					LogWarning("Failed to read from package source. Switching to next stream.", exception);
					_currentStream?.Dispose();
					_currentStream = null;
				}
			}
			while (!streamReadResult.HasValue);
			return streamReadResult.Value;
		}

		private IPackageSourceStreamDisposable GetNextSourceStream()
		{
			AppRemotePackageSourceInfo info = Request.SourcesInfo[GetNextSourceStreamIndex()];
			PackageSourceStreamRequest request = new PackageSourceStreamRequest(info, new BytesCount(_currentPosition));
			IPackageSourceStreamDisposable packageSourceStreamDisposable = GetPackageSourceStream(request);
			Assert.IsNotNullFromDelegate(packageSourceStreamDisposable);
			return packageSourceStreamDisposable;
		}

		private int GetNextSourceStreamIndex()
		{
			_currentSourceIndex++;
			bool v = _currentSourceIndex >= 0;
			Assert.IsTrue(v, "isCurrentResourceIndexEqualOrMoreThan0");
			if (_currentSourceIndex >= Request.SourcesInfo.Length)
			{
				_currentSourceIndex = 0;
			}
			return _currentSourceIndex;
		}

		public StreamReadResult Read(Buffer<byte> buffer, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
		{
			_disposalHandler.ThrowIfDisposed();
			Arg.IsValid(buffer, "buffer");
			try
			{
				return ExecuteRead(buffer, cancellationToken);
			}
			catch (InvalidAppAuthKey e)
			{
				if (onInvalidAppAuthKey == null)
				{
					throw;
				}
				object obj = onInvalidAppAuthKey(e);
				if (obj == null)
				{
					return default;
				}
				return (StreamReadResult)obj;
			}
			catch (ExpiredAppAuthKey e2)
			{
				if (onExpiredAppAuthKey == null)
				{
					throw;
				}
				object obj2 = onExpiredAppAuthKey(e2);
				if (obj2 == null)
				{
					return default;
				}
				return (StreamReadResult)obj2;
			}
			catch (OperationCanceledException)
			{
				LogInfo("PackageMultisourceStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageMultisourceStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageMultisourceStream::Read finished with fatal error.", ex2);
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
				LogInfo("PackageMultisourceStream::GetBufferedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageMultisourceStream::GetBufferedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageMultisourceStream::GetBufferedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public PackageMultisourceStream(PackageMultisourceStreamRequest request, GetPackageSourceStreamDelegate getPackageSourceStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(request, "request");
			Arg.IsNotNull(getPackageSourceStream, "getPackageSourceStream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Request = request;
			GetPackageSourceStream = getPackageSourceStream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~PackageMultisourceStream()
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

	public GetPackageSourceStreamDelegate GetPackageSourceStream { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageMultisourceStreamDisposable Execute(PackageMultisourceStreamRequest request)
	{
		return NewPackageMultisourceStream(request);
	}

	private PackageMultisourceStream NewPackageMultisourceStream(PackageMultisourceStreamRequest request)
	{
		return new PackageMultisourceStream(request, GetPackageSourceStream, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPackageMultisourceStream(GetPackageSourceStreamDelegate getPackageSourceStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getPackageSourceStream);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetPackageSourceStream = getPackageSourceStream;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageMultisourceStreamDisposable DelegateExecute(PackageMultisourceStreamRequest request)
	{
		Arg.IsValid(request, "request");
		LogInfo("Executing GetPackageMultisourceStream...");
		PackageMultisourceStreamRequest packageMultisourceStreamRequest = request;
		LogTrace("request = " + packageMultisourceStreamRequest.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageMultisourceStreamDisposable packageMultisourceStreamDisposable = Execute(request);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageMultisourceStreamDisposable);
			LogInfo("GetPackageMultisourceStream finished: success.");
			return packageMultisourceStreamDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackageMultisourceStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageMultisourceStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageMultisourceStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageMultisourceStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackageMultisourceStream(context.Resolve<GetPackageSourceStreamDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
