using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using SharpCompress.Compressors.Xz;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPack1FileStream
{
	private class Pack1FileStreamAdapter : Stream
	{
		private readonly IPack1FileSourceStream _sourceStream;

		public CancellationToken ReadCancellationToken;

		public override long Length
		{
			get
			{
				throw new UnpredictedCondition("Get Length shouldn't be called.");
			}
		}

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Position
		{
			get
			{
				throw new UnpredictedCondition("Get Position shouldn't be called.");
			}
			set
			{
				throw new UnpredictedCondition("Set Position shouldn't be called.");
			}
		}

		public Pack1FileStreamAdapter(IPack1FileSourceStream sourceStream)
		{
			_sourceStream = sourceStream;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _sourceStream.Read(new Buffer<byte>(buffer, new BufferOffset(offset), new BufferLength(count)), ReadCancellationToken, null, null).Length;
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new UnpredictedCondition("Seek shouldn't be called.");
		}

		public override void SetLength(long value)
		{
			throw new UnpredictedCondition("SetLength shouldn't be called.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new UnpredictedCondition("Write shouldn't be called.");
		}
	}

	private class CryptoStreamWrapper : Stream
	{
		private readonly CryptoStream _cryptoStream;

		private long _position;

		public override long Length => _cryptoStream.Length;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Position
		{
			get
			{
				return _position;
			}
			set
			{
				throw new UnpredictedCondition("Set Position shouldn't be called.");
			}
		}

		public CryptoStreamWrapper(CryptoStream cryptoStream)
		{
			_cryptoStream = cryptoStream;
			_position = 0L;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = _cryptoStream.Read(buffer, offset, count);
			_position += num;
			return num;
		}

		public override void Flush()
		{
			throw new UnpredictedCondition("Flush shouldn't be called.");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new UnpredictedCondition("Seek shouldn't be called.");
		}

		public override void SetLength(long value)
		{
			throw new UnpredictedCondition("SetLength shouldn't be called.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new UnpredictedCondition("Write shouldn't be called.");
		}
	}

	private class Pack1FileStream : IDisposable, IPackageFileStreamDisposable, IPackageFileStream, IBaseDisposable
	{
		private CryptoStream _cryptoStream;

		private Pack1FileStreamAdapter _pack1FileStreamAdapter;

		private Stream _compressionStream;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "Pack1FileStream";

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private IPack1FileSourceStream SourceStream { get; }

		private AppRemotePack1Encryption Encryption { get; }

		private AppRemotePack1CompressionMethod CompressionMethod { get; }

		private AppRemotePack1UnpackPassword Password { get; }

		private void Constructor()
		{
			Assert.IsNotNull(Encryption.Aes);
			byte[] iV = Convert.FromBase64String(Encryption.Aes.Value.InitializationVector);
			byte[] key;
			using (SHA256 sHA = SHA256.Create())
			{
				Assert.IsNotNull(sHA);
				key = sHA.ComputeHash(Encoding.ASCII.GetBytes(Password));
			}
			AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider
			{
				IV = iV,
				Key = key
			};
			ICryptoTransform transform = aesCryptoServiceProvider.CreateDecryptor(aesCryptoServiceProvider.Key, aesCryptoServiceProvider.IV);
			_pack1FileStreamAdapter = new Pack1FileStreamAdapter(SourceStream);
			_cryptoStream = new CryptoStream(_pack1FileStreamAdapter, transform, CryptoStreamMode.Read);
			switch (CompressionMethod)
			{
			case AppRemotePack1CompressionMethod.Gzip:
				_compressionStream = new GZipStream(_cryptoStream, CompressionMode.Decompress);
				break;
			case AppRemotePack1CompressionMethod.Lzma2:
				_compressionStream = new XZStream(new CryptoStreamWrapper(_cryptoStream));
				break;
			default:
				throw new UnpredictedCondition($"Unsupported compression: {CompressionMethod}");
			}
		}

		private void DisposeManaged()
		{
			Assert.IsNotNull(_cryptoStream);
			Assert.IsNotNull(_compressionStream);
			try
			{
				_cryptoStream.Dispose();
			}
			catch (CryptographicException)
			{
			}
			_compressionStream.Dispose();
		}

		private void DisposeUnmanaged()
		{
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(_compressionStream);
			_pack1FileStreamAdapter.ReadCancellationToken = cancellationToken;
			int v = _compressionStream.Read(buffer.Array, buffer.Offset, buffer.Length);
			return new StreamReadResult(new StreamReadLength(v));
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
				LogInfo("Pack1FileStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("Pack1FileStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("Pack1FileStream::Read finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public Pack1FileStream(IPack1FileSourceStream sourceStream, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsNotNull(sourceStream, "sourceStream");
			Arg.IsValid(encryption, "encryption");
			Arg.IsValid(password, "password");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			SourceStream = sourceStream;
			Encryption = encryption;
			CompressionMethod = compressionMethod;
			Password = password;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~Pack1FileStream()
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

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageFileStreamDisposable Execute(IPack1FileSourceStream sourceStream, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password)
	{
		return NewPack1FileStream(sourceStream, encryption, compressionMethod, password);
	}

	private Pack1FileStream NewPack1FileStream(IPack1FileSourceStream sourceStream, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password)
	{
		return new Pack1FileStream(sourceStream, encryption, compressionMethod, password, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPack1FileStream(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageFileStreamDisposable DelegateExecute(IPack1FileSourceStream sourceStream, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password)
	{
		Arg.IsNotNull(sourceStream, "sourceStream");
		Arg.IsValid(encryption, "encryption");
		Arg.IsValid(password, "password");
		LogInfo("Executing GetPack1FileStream...");
		LogTrace("sourceStream = " + sourceStream);
		AppRemotePack1Encryption appRemotePack1Encryption = encryption;
		LogTrace("encryption = " + appRemotePack1Encryption.ToString());
		LogTrace("compressionMethod = " + compressionMethod);
		LogTrace("password = " + password);
		try
		{
			BeginLogRegion(enabled: true);
			IPackageFileStreamDisposable packageFileStreamDisposable = Execute(sourceStream, encryption, compressionMethod, password);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageFileStreamDisposable);
			LogInfo("GetPack1FileStream finished: success.");
			return packageFileStreamDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPack1FileStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPack1FileStream(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
