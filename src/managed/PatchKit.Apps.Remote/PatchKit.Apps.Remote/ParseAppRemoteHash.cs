using System;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Remote;

public class ParseAppRemoteHash
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Hash Execute(Text value)
	{
		string text = value.Value;
		while (text.Length != 8)
		{
			text = "0" + text;
		}
		byte[] array = new byte[text.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			int num = array.Length - 1 - i;
			array[num] = Convert.ToByte(text.Substring(i * 2, 2), 16);
		}
		return new Hash(array.AsImmutable());
	}

	public ParseAppRemoteHash(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Hash DelegateExecute(Text value)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing ParseAppRemoteHash...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: false);
			Hash hash = Execute(value);
			EndLogRegion(enabled: false);
			Hash hash2 = hash;
			LogTrace("result = " + hash2.ToString());
			LogInfo("ParseAppRemoteHash finished: success.");
			return hash;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("ParseAppRemoteHash has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("ParseAppRemoteHash failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("ParseAppRemoteHash finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ParseAppRemoteHashDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ParseAppRemoteHash(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
