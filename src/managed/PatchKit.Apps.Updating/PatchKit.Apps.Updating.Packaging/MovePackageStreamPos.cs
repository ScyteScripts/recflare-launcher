using System;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public class MovePackageStreamPos
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(IPackageStream stream, BytesCount targetPos, CancellationToken cancellationToken)
	{
		BytesCount currentPosition = stream.GetCurrentPosition();
		if ((long)currentPosition > (long)targetPos || (long)targetPos >= (long)stream.Length)
		{
			throw new PackageStreamPosOutOfRange(currentPosition, targetPos, stream.Length);
		}
		if ((long)currentPosition == (long)targetPos)
		{
			LogInfo("Stream already at target position.");
			return;
		}
		BufferLength bufferLength = new BufferLength(4194304);
		byte[] array = new byte[(int)bufferLength];
		while ((long)(currentPosition = stream.GetCurrentPosition()) < (long)targetPos)
		{
			int v = (int)Math.Min(targetPos - currentPosition, (int)bufferLength);
			Buffer<byte> buffer = new Buffer<byte>(array, new BufferOffset(0), new BufferLength(v));
			bool isNotEndOfStream = stream.Read(buffer, cancellationToken, null, null).IsNotEndOfStream;
			Assert.IsTrue(isNotEndOfStream, "isNotEndOfStream");
		}
		bool v2 = (long)stream.GetCurrentPosition() == (long)targetPos;
		Assert.IsTrue(v2, "doesStreamCurrentPosEqualTargetPos");
	}

	public MovePackageStreamPos(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(IPackageStream stream, BytesCount targetPos, CancellationToken cancellationToken, HandleExceptionDelegate<PackageStreamPosOutOfRange> onPackageStreamPosOutOfRange, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsNotNull(stream, "stream");
		Arg.IsValid(targetPos, "targetPos");
		LogInfo("Executing MovePackageStreamPos...");
		LogTrace("stream = " + stream);
		BytesCount bytesCount = targetPos;
		LogTrace("targetPos = " + bytesCount.ToString());
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(stream, targetPos, cancellationToken);
			EndLogRegion(enabled: true);
			LogInfo("MovePackageStreamPos finished: success.");
			return null;
		}
		catch (PackageStreamPosOutOfRange e)
		{
			EndLogRegion(enabled: true);
			LogInfo("MovePackageStreamPos finished: PackageStreamPosOutOfRange.");
			if (onPackageStreamPosOutOfRange == null)
			{
				throw;
			}
			onPackageStreamPosOutOfRange(e);
			return null;
		}
		catch (InvalidAppAuthKey e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("MovePackageStreamPos finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			onInvalidAppAuthKey(e2);
			return null;
		}
		catch (ExpiredAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("MovePackageStreamPos finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			onExpiredAppAuthKey(e3);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("MovePackageStreamPos has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("MovePackageStreamPos failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("MovePackageStreamPos finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public MovePackageStreamPosDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new MovePackageStreamPos(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
