using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackageWithoutChunksStream
{
	private class PackageWithoutChunksStream : IDisposable, IPackageStreamDisposable, IPackageStream, IBaseDisposable
	{
		private IPackageMultisourceStreamDisposable _stream;

		private long _currentPosition;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "PackageWithoutChunksStream";

		private GetPackageMultisourceStreamDelegate GetPackageMultisourceStream { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public BytesCount Length { get; }

		private ImmutableArray<AppRemotePackageSourceInfo> SourcesInfo { get; }

		private void Constructor()
		{
			_currentPosition = 0L;
			_stream = GetPackageMultisourceStream(new PackageMultisourceStreamRequest(SourcesInfo, new BytesCount(0L)));
		}

		private void DisposeManaged()
		{
			_stream?.Dispose();
			_stream = null;
		}

		private void DisposeUnmanaged()
		{
		}

		private BytesCount ExecuteGetBufferedBytes()
		{
			if (_stream == null)
			{
				return new BytesCount(0L);
			}
			return _stream.GetBufferedBytes();
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer, CancellationToken cancellationToken)
		{
			StreamReadResult result = _stream.Read(buffer, cancellationToken, null, null);
			_currentPosition += (int)result.Length;
			return result;
		}

		private BytesCount ExecuteGetCurrentPosition()
		{
			return new BytesCount(_currentPosition);
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
				LogInfo("PackageWithoutChunksStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageWithoutChunksStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageWithoutChunksStream::Read finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public BytesCount GetCurrentPosition()
		{
			_disposalHandler.ThrowIfDisposed();
			try
			{
				return ExecuteGetCurrentPosition();
			}
			catch (OperationCanceledException)
			{
				LogInfo("PackageWithoutChunksStream::GetCurrentPosition has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageWithoutChunksStream::GetCurrentPosition failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageWithoutChunksStream::GetCurrentPosition finished with fatal error.", ex2);
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
				LogInfo("PackageWithoutChunksStream::GetBufferedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageWithoutChunksStream::GetBufferedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageWithoutChunksStream::GetBufferedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public PackageWithoutChunksStream(BytesCount length, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, GetPackageMultisourceStreamDelegate getPackageMultisourceStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(length, "length");
			Arg.IsValid(sourcesInfo, "sourcesInfo");
			Arg.IsNotNull(getPackageMultisourceStream, "getPackageMultisourceStream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Length = length;
			SourcesInfo = sourcesInfo;
			GetPackageMultisourceStream = getPackageMultisourceStream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~PackageWithoutChunksStream()
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

	public GetPackageMultisourceStreamDelegate GetPackageMultisourceStream { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageStreamDisposable Execute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, BytesCount size)
	{
		return NewPackageWithoutChunksStream(size, sourcesInfo);
	}

	private PackageWithoutChunksStream NewPackageWithoutChunksStream(BytesCount length, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo)
	{
		return new PackageWithoutChunksStream(length, sourcesInfo, GetPackageMultisourceStream, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPackageWithoutChunksStream(GetPackageMultisourceStreamDelegate getPackageMultisourceStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getPackageMultisourceStream);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetPackageMultisourceStream = getPackageMultisourceStream;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageStreamDisposable DelegateExecute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, BytesCount size)
	{
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(size, "size");
		LogInfo("Executing GetPackageWithoutChunksStream...");
		ImmutableArray<AppRemotePackageSourceInfo> immutableArray = sourcesInfo;
		LogTrace("sourcesInfo = " + immutableArray.ToString());
		BytesCount bytesCount = size;
		LogTrace("size = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageStreamDisposable packageStreamDisposable = Execute(sourcesInfo, size);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageStreamDisposable);
			LogInfo("GetPackageWithoutChunksStream finished: success.");
			return packageStreamDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackageWithoutChunksStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageWithoutChunksStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageWithoutChunksStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageWithoutChunksStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackageWithoutChunksStream(context.Resolve<GetPackageMultisourceStreamDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
