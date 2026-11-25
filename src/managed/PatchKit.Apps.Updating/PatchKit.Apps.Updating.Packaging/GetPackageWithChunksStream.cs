using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackageWithChunksStream
{
	private class PackageWithChunksStream : IDisposable, IPackageStreamDisposable, IPackageStream, IBaseDisposable
	{
		private IPackageMultisourceStreamDisposable _currentStream;

		private int _currentChunkIndex = -1;

		private byte[] _chunkBuffer;

		private int _chunkBufferLength;

		private int _chunkBufferIndex;

		private int _lastChunkBufferLength;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "PackageWithChunksStream";

		private GetBufferxxHashDelegate GetBufferxxHash { get; }

		private GetPackageMultisourceStreamDelegate GetPackageMultisourceStream { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public BytesCount Length { get; }

		private ImmutableArray<AppRemotePackageSourceInfo> SourcesInfo { get; }

		private AppRemotePackageChunksInfo ChunksInfo { get; }

		private void Constructor()
		{
			_chunkBuffer = new byte[ChunksInfo.Size.Value];
			_chunkBufferIndex = 0;
			_chunkBufferLength = 0;
			_lastChunkBufferLength = (int)((long)Length - (ChunksInfo.Hashes.Length - 1) * (long)ChunksInfo.Size);
		}

		private void DisposeManaged()
		{
			_currentStream?.Dispose();
			_currentStream = null;
		}

		private void DisposeUnmanaged()
		{
		}

		private void FillChunkBuffer(CancellationToken cancellationToken)
		{
			bool v = _chunkBufferIndex == _chunkBufferLength;
			Assert.IsTrue(v, "isChunkBufferFullyUsed");
			_currentChunkIndex++;
			bool flag = false;
			while (!flag)
			{
				if (_currentStream == null)
				{
					_currentStream = GetPackageMultisourceStream(new PackageMultisourceStreamRequest(SourcesInfo, new BytesCount(_currentChunkIndex * (long)ChunksInfo.Size)));
				}
				int i = 0;
				bool flag2 = _currentChunkIndex == ChunksInfo.Hashes.Length - 1;
				int v2;
				for (_chunkBufferLength = (int)(flag2 ? _lastChunkBufferLength : ((long)ChunksInfo.Size)); i < _chunkBufferLength; i += (int)_currentStream.Read(new Buffer<byte>(_chunkBuffer, new BufferOffset(i), new BufferLength(v2)), cancellationToken, null, null).Length)
				{
					v2 = _chunkBufferLength - i;
				}
				bool v3 = i == _chunkBufferLength;
				Assert.IsTrue(v3, "doesLengthEqualChunkBufferLength");
				Hash hash = GetBufferxxHash(new Buffer<byte>(_chunkBuffer, new BufferOffset(0), new BufferLength(_chunkBufferLength)), new xxHashSeed(42));
				flag = hash.Equals(ChunksInfo.Hashes[_currentChunkIndex]);
				if (!flag)
				{
					LogWarning("Invalid chunk hash. Expected " + ChunksInfo.Hashes[_currentChunkIndex].BytesAsString + " but got " + hash.BytesAsString + ". Redownloading...");
					_currentStream?.Dispose();
					_currentStream = null;
				}
			}
			_chunkBufferIndex = 0;
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
			int i;
			int val2;
			for (i = 0; i < (int)buffer.Length; i += val2)
			{
				if (_chunkBufferIndex == _chunkBufferLength)
				{
					if (_currentChunkIndex == ChunksInfo.Hashes.Length - 1)
					{
						break;
					}
					FillChunkBuffer(cancellationToken);
				}
				int destinationIndex = (int)buffer.Offset + i;
				int val = (int)buffer.Length - i;
				val2 = _chunkBufferLength - _chunkBufferIndex;
				val2 = Math.Min(val2, val);
				Array.Copy(_chunkBuffer, _chunkBufferIndex, buffer.Array, destinationIndex, val2);
				_chunkBufferIndex += val2;
			}
			if (i == 0)
			{
				return StreamReadResult.EndOfStream;
			}
			return new StreamReadResult(new StreamReadLength(i));
		}

		private BytesCount ExecuteGetCurrentPosition()
		{
			if (_currentChunkIndex == -1)
			{
				return new BytesCount(0L);
			}
			return new BytesCount(_currentChunkIndex * ChunksInfo.Size.Value + _chunkBufferIndex);
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
				LogInfo("PackageWithChunksStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageWithChunksStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageWithChunksStream::Read finished with fatal error.", ex2);
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
				LogInfo("PackageWithChunksStream::GetCurrentPosition has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageWithChunksStream::GetCurrentPosition failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageWithChunksStream::GetCurrentPosition finished with fatal error.", ex2);
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
				LogInfo("PackageWithChunksStream::GetBufferedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageWithChunksStream::GetBufferedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageWithChunksStream::GetBufferedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public PackageWithChunksStream(BytesCount length, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo chunksInfo, GetBufferxxHashDelegate getBufferxxHash, GetPackageMultisourceStreamDelegate getPackageMultisourceStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(length, "length");
			Arg.IsValid(sourcesInfo, "sourcesInfo");
			Arg.IsValid(chunksInfo, "chunksInfo");
			Arg.IsNotNull(getBufferxxHash, "getBufferxxHash");
			Arg.IsNotNull(getPackageMultisourceStream, "getPackageMultisourceStream");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Length = length;
			SourcesInfo = sourcesInfo;
			ChunksInfo = chunksInfo;
			GetBufferxxHash = getBufferxxHash;
			GetPackageMultisourceStream = getPackageMultisourceStream;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~PackageWithChunksStream()
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

	public GetBufferxxHashDelegate GetBufferxxHash { get; }

	public GetPackageMultisourceStreamDelegate GetPackageMultisourceStream { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageStreamDisposable Execute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo chunksInfo, BytesCount size)
	{
		return NewPackageWithChunksStream(size, sourcesInfo, chunksInfo);
	}

	private PackageWithChunksStream NewPackageWithChunksStream(BytesCount length, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo chunksInfo)
	{
		return new PackageWithChunksStream(length, sourcesInfo, chunksInfo, GetBufferxxHash, GetPackageMultisourceStream, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPackageWithChunksStream(GetBufferxxHashDelegate getBufferxxHash, GetPackageMultisourceStreamDelegate getPackageMultisourceStream, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getBufferxxHash);
		Assert.IsNotNull(getPackageMultisourceStream);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetBufferxxHash = getBufferxxHash;
		GetPackageMultisourceStream = getPackageMultisourceStream;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageStreamDisposable DelegateExecute(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo chunksInfo, BytesCount size)
	{
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(chunksInfo, "chunksInfo");
		Arg.IsValid(size, "size");
		LogInfo("Executing GetPackageWithChunksStream...");
		ImmutableArray<AppRemotePackageSourceInfo> immutableArray = sourcesInfo;
		LogTrace("sourcesInfo = " + immutableArray.ToString());
		AppRemotePackageChunksInfo appRemotePackageChunksInfo = chunksInfo;
		LogTrace("chunksInfo = " + appRemotePackageChunksInfo.ToString());
		BytesCount bytesCount = size;
		LogTrace("size = " + bytesCount.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageStreamDisposable packageStreamDisposable = Execute(sourcesInfo, chunksInfo, size);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageStreamDisposable);
			LogInfo("GetPackageWithChunksStream finished: success.");
			return packageStreamDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackageWithChunksStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageWithChunksStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageWithChunksStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageWithChunksStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackageWithChunksStream(context.Resolve<GetBufferxxHashDelegate>(), context.Resolve<GetPackageMultisourceStreamDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
