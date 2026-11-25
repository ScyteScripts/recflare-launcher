using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Network;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPackageSourcePartStream
{
	private class PackageSourcePartStream : IDisposable, IPackageSourcePartStreamDisposable, IPackageSourcePartStream, IBaseDisposable
	{
		private IBufferedHttpStreamedResponseDisposable _response;

		private List<IDisposable> _attachedDisposables;

		private readonly ObjectDisposalHandler _disposalHandler;

		private const string ClassName = "PackageSourcePartStream";

		private GetBufferedHttpStreamedResponseDelegate GetBufferedHttpStreamedResponse { get; }

		private LogDelegate Log { get; }

		private BeginLogRegionDelegate BeginLogRegion { get; }

		private EndLogRegionDelegate EndLogRegion { get; }

		private PackageSourcePartStreamRequest Request { get; }

		private void Constructor()
		{
		}

		private void DisposeManaged()
		{
			_response?.Dispose();
		}

		private void DisposeUnmanaged()
		{
		}

		private BytesCount ExecuteGetBufferedBytes()
		{
			if (_response == null)
			{
				return new BytesCount(0L);
			}
			return _response.BodyStream.GetBufferedBytes();
		}

		private StreamReadResult ExecuteRead(Buffer<byte> buffer, CancellationToken cancellationToken)
		{
			if (_response == null)
			{
				long start = Request.Info.Offset + Request.Offset;
				long value = (long)(Request.Info.Offset + Request.Info.Size) - 1;
				ImmutableArray<PatchKit.Network.HttpRequestHeader> headers = ImmutableArray<PatchKit.Network.HttpRequestHeader>.Empty;
				if (Request.Info.AuthHeaderValue.HasValue)
				{
					headers = new PatchKit.Network.HttpRequestHeader[1]
					{
						new PatchKit.Network.HttpRequestHeader(new Text("Authorization"), Request.Info.AuthHeaderValue.Value)
					}.AsImmutable();
				}
				_response = GetBufferedHttpStreamedResponse(new HttpRequestInfo(Request.Info.Address, HttpRequestMethod.Get, null, new HttpRequestBytesRange(start, value), headers), new PatchKit.Core.Timeout(TimeSpan.FromSeconds(15.0)), (ConnectionFailure e) =>
				{
					throw new PackageSourceStreamReadFailure(e);
				});
				Assert.IsNotNullFromDelegate(_response);
				if (_response.StatusCode == HttpStatusCode.Unauthorized)
				{
					throw new InvalidAppAuthKey();
				}
				if (_response.StatusCode == HttpStatusCode.Gone)
				{
					throw new ExpiredAppAuthKey();
				}
				if (!IsStatusCodeOk(_response.StatusCode))
				{
					throw new PackageSourceStreamReadFailure();
				}
			}
			return _response.BodyStream.Read(buffer, (ConnectionFailure e) =>
			{
				throw new PackageSourceStreamReadFailure(e);
			});
		}

		private static bool IsStatusCodeOk(HttpStatusCode statusCode)
		{
			return statusCode >= HttpStatusCode.OK && statusCode <= HttpStatusCode.MultipleChoices;
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
				LogInfo("PackageSourcePartStream::Read has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageSourcePartStream::Read failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageSourcePartStream::Read finished with fatal error.", ex2);
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
				LogInfo("PackageSourcePartStream::GetBufferedBytes has been cancelled.");
				throw;
			}
			catch (FunctionFatalError)
			{
				LogError("PackageSourcePartStream::GetBufferedBytes failed due to other function failure.");
				throw;
			}
			catch (Exception ex2)
			{
				LogError("PackageSourcePartStream::GetBufferedBytes finished with fatal error.", ex2);
				throw new FunctionFatalError(ex2);
			}
		}

		public PackageSourcePartStream(PackageSourcePartStreamRequest request, GetBufferedHttpStreamedResponseDelegate getBufferedHttpStreamedResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
		{
			Arg.IsValid(request, "request");
			Arg.IsNotNull(getBufferedHttpStreamedResponse, "getBufferedHttpStreamedResponse");
			Arg.IsNotNull(log, "log");
			Arg.IsNotNull(beginLogRegion, "beginLogRegion");
			Arg.IsNotNull(endLogRegion, "endLogRegion");
			Request = request;
			GetBufferedHttpStreamedResponse = getBufferedHttpStreamedResponse;
			Log = log;
			BeginLogRegion = beginLogRegion;
			EndLogRegion = endLogRegion;
			_disposalHandler = new ObjectDisposalHandler(this);
			Constructor();
		}

		~PackageSourcePartStream()
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

	public GetBufferedHttpStreamedResponseDelegate GetBufferedHttpStreamedResponse { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageSourcePartStreamDisposable Execute(PackageSourcePartStreamRequest request)
	{
		return NewPackageSourcePartStream(request);
	}

	private PackageSourcePartStream NewPackageSourcePartStream(PackageSourcePartStreamRequest request)
	{
		return new PackageSourcePartStream(request, GetBufferedHttpStreamedResponse, Log, BeginLogRegion, EndLogRegion);
	}

	public GetPackageSourcePartStream(GetBufferedHttpStreamedResponseDelegate getBufferedHttpStreamedResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getBufferedHttpStreamedResponse);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetBufferedHttpStreamedResponse = getBufferedHttpStreamedResponse;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageSourcePartStreamDisposable DelegateExecute(PackageSourcePartStreamRequest request)
	{
		Arg.IsValid(request, "request");
		LogInfo("Executing GetPackageSourcePartStream...");
		PackageSourcePartStreamRequest packageSourcePartStreamRequest = request;
		LogTrace("request = " + packageSourcePartStreamRequest.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			IPackageSourcePartStreamDisposable packageSourcePartStreamDisposable = Execute(request);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageSourcePartStreamDisposable);
			LogInfo("GetPackageSourcePartStream finished: success.");
			return packageSourcePartStreamDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPackageSourcePartStream has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageSourcePartStream failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPackageSourcePartStream finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPackageSourcePartStreamDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPackageSourcePartStream(context.Resolve<GetBufferedHttpStreamedResponseDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
