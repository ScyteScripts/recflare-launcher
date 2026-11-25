using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPack1FileSourceStream
{
	private class Pack1FileSourceStream : IDisposable, IPack1FileSourceStreamDisposable, IPack1FileSourceStream, IBaseDisposable
	{
		private long _bytesLeft;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "Pack1FileSourceStream";

		private MovePackageStreamPosDelegate MovePackageStreamPos { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		public BytesCount Length { get; }

		private IPackageStream PackageStream { get; }

		private void Constructor()
		{
			_bytesLeft = Length;
		}

		private void DisposeManaged()
		{
		}

		private void DisposeUnmanaged()
		{
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer, CancellationToken cancellationToken)
		{
			bool v = _bytesLeft >= 0;
			Assert.IsTrue(v, "bytesLeftEqualOrMoreThan0");
			if (_bytesLeft == 0)
			{
				return StreamReadResult.EndOfStream;
			}
			BufferLength newLength = new BufferLength((int)Math.Min(_bytesLeft, (int)buffer.Length));
			StreamReadResult result = PackageStream.Read(buffer.ModifyLength(newLength), cancellationToken, null, null);
			bool isEndOfStream = result.IsEndOfStream;
			Assert.IsFalse(isEndOfStream, "isEndOfStream");
			_bytesLeft -= (int)result.Length;
			return result;
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
				LogInfo("Pack1FileSourceStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("Pack1FileSourceStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("Pack1FileSourceStream::Read finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public Pack1FileSourceStream(BytesCount length, IPackageStream packageStream, MovePackageStreamPosDelegate movePackageStreamPos, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(length, "length");
			Arg.IsNotNull(packageStream, "packageStream");
			Arg.IsNotNull(movePackageStreamPos, "movePackageStreamPos");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Length = length;
			PackageStream = packageStream;
			MovePackageStreamPos = movePackageStreamPos;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~Pack1FileSourceStream()
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

	public MovePackageStreamPosDelegate MovePackageStreamPos { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPack1FileSourceStreamDisposable Execute(IPackageStream packageStream, BytesCount startPos, BytesCount length, CancellationToken cancellationToken)
	{
		if ((long)(startPos + length) > (long)packageStream.Length)
		{
			throw new PackageFileSourceStreamOutOfRange(packageStream.GetCurrentPosition(), packageStream.Length, startPos, length);
		}
		MovePackageStreamPos(packageStream, startPos, cancellationToken, (PackageStreamPosOutOfRange e) =>
		{
			throw new PackageFileSourceStreamOutOfRange(packageStream.GetCurrentPosition(), packageStream.Length, startPos, length);
		}, null, null);
		return NewPack1FileSourceStream(length, packageStream);
	}

	private Pack1FileSourceStream NewPack1FileSourceStream(BytesCount length, IPackageStream packageStream)
	{
		return new Pack1FileSourceStream(length, packageStream, MovePackageStreamPos, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPack1FileSourceStream(MovePackageStreamPosDelegate movePackageStreamPos, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(movePackageStreamPos);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		MovePackageStreamPos = movePackageStreamPos;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPack1FileSourceStreamDisposable DelegateExecute(IPackageStream packageStream, BytesCount startPos, BytesCount length, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<PackageFileSourceStreamOutOfRange> onPackageFileSourceStreamOutOfRange, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(packageStream, "packageStream");
		Arg.IsValid(startPos, "startPos");
		Arg.IsValid(length, "length");
		LogInfo("Executing GetPack1FileSourceStream...");
		LogTrace("packageStream = " + packageStream);
		BytesCount bytesCount = startPos;
		LogTrace("startPos = " + bytesCount.ToString());
		bytesCount = length;
		LogTrace("length = " + bytesCount.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			IPack1FileSourceStreamDisposable pack1FileSourceStreamDisposable = Execute(packageStream, startPos, length, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + pack1FileSourceStreamDisposable);
			LogInfo("GetPack1FileSourceStream finished: success.");
			return pack1FileSourceStreamDisposable;
		}
		catch (PackageFileSourceStreamOutOfRange e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileSourceStream finished: PackageFileSourceStreamOutOfRange.");
			if (onPackageFileSourceStreamOutOfRange == null)
			{
				throw;
			}
			object obj = onPackageFileSourceStreamOutOfRange(e);
			if (obj == null)
			{
				return null;
			}
			return (IPack1FileSourceStreamDisposable)obj;
		}
		catch (InvalidAppAuthKey e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileSourceStream finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj2 = onInvalidAppAuthKey(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (IPack1FileSourceStreamDisposable)obj2;
		}
		catch (ExpiredAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileSourceStream finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj3 = onExpiredAppAuthKey(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (IPack1FileSourceStreamDisposable)obj3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileSourceStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileSourceStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileSourceStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPack1FileSourceStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPack1FileSourceStream(context.Resolve<MovePackageStreamPosDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
