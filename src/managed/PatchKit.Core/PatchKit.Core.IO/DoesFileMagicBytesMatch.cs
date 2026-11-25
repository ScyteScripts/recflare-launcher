using System;
using System.IO;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class DoesFileMagicBytesMatch
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(Path path, MagicBytes magicBytes)
	{
		try
		{
			using BinaryReader binaryReader = new BinaryReader(new FileStream(path, FileMode.Open));
			try
			{
				byte[] array = new byte[magicBytes.Bytes.Length];
				binaryReader.BaseStream.Seek(magicBytes.Offset, SeekOrigin.Begin);
				binaryReader.Read(array, 0, array.Length);
				return magicBytes.Bytes.SequenceEqual(array);
			}
			catch
			{
				return false;
			}
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

	public DoesFileMagicBytesMatch(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(Path path, MagicBytes magicBytes, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(path, "path");
		Arg.IsValid(magicBytes, "magicBytes");
		LogInfo("Executing DoesFileMagicBytesMatch...");
		LogTrace("path = " + path);
		MagicBytes magicBytes2 = magicBytes;
		LogTrace("magicBytes = " + magicBytes2.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(path, magicBytes);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("DoesFileMagicBytesMatch finished: success.");
			return result;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesFileMagicBytesMatch finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return false;
			}
			return (bool)obj;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesFileMagicBytesMatch finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingFile(e2);
			if (obj2 == null)
			{
				return false;
			}
			return (bool)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DoesFileMagicBytesMatch has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DoesFileMagicBytesMatch failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DoesFileMagicBytesMatch finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DoesFileMagicBytesMatchDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DoesFileMagicBytesMatch(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
