using System;
using Autofac;
using PatchKit.Core.CSharp;
using Standart.Hash.xxHash;

namespace PatchKit.Core.IO.Hashing;

public class GetBufferxxHash
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Hash Execute(Buffer<byte> buffer, xxHashSeed seed)
	{
		byte[] array;
		int length;
		if ((int)buffer.Offset != 0)
		{
			array = new byte[(int)buffer.Length];
			length = buffer.Length;
			Array.Copy(buffer.Array, buffer.Offset, array, 0, buffer.Length);
		}
		else
		{
			array = buffer.Array;
			length = buffer.Length;
		}
		uint value = xxHash32.ComputeHash(array, length, (uint)seed.Value);
		byte[] bytes = BitConverter.GetBytes(value);
		return new Hash(bytes.AsImmutable());
	}

	public GetBufferxxHash(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Hash DelegateExecute(Buffer<byte> buffer, xxHashSeed seed)
	{
		Arg.IsValid(buffer, "buffer");
		Arg.IsValid(seed, "seed");
		try
		{
			return Execute(buffer, seed);
		}
		catch (OperationCanceledException)
		{
			LogInfo("GetBufferxxHash has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			LogError("GetBufferxxHash failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			LogError("GetBufferxxHash finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetBufferxxHashDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetBufferxxHash(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
