using System;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;
using Standart.Hash.xxHash;

namespace PatchKit.Core.IO.Hashing;

public class GetFilexxHash
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Hash Execute(Path path, xxHashSeed seed)
	{
		try
		{
			using FileStream stream = File.OpenRead(path);
			uint value = xxHash32.ComputeHash(stream, 4096, (uint)seed.Value);
			byte[] bytes = BitConverter.GetBytes(value);
			return new Hash(bytes.AsImmutable());
		}
		catch (DirectoryNotFoundException inner)
		{
			throw new NotExistingFile(path, inner);
		}
		catch (FileNotFoundException inner2)
		{
			throw new NotExistingFile(path, inner2);
		}
		catch (UnauthorizedAccessException inner3)
		{
			throw new UnauthorizedAccess(inner3);
		}
	}

	public GetFilexxHash(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Hash DelegateExecute(Path path, xxHashSeed seed, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(seed, "seed");
		LogInfo("Executing GetFilexxHash...");
		LogTrace("path = " + path);
		xxHashSeed xxHashSeed2 = seed;
		LogTrace("seed = " + xxHashSeed2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Hash hash = Execute(path, seed);
			EndLogRegion(enabled: true);
			Hash hash2 = hash;
			LogTrace("result = " + hash2.ToString());
			LogInfo("GetFilexxHash finished: success.");
			return hash;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilexxHash finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (Hash)obj;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilexxHash finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingFile(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (Hash)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilexxHash has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilexxHash failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilexxHash finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilexxHashDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilexxHash(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
