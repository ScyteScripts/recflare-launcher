using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class IsFileOSXExecutable
{
	private static readonly MagicBytes MachO32 = new MagicBytes(new byte[4] { 254, 237, 250, 206 }.AsImmutable(), new BytesCount(4096L));

	private static readonly MagicBytes MachO64 = new MagicBytes(new byte[4] { 254, 237, 250, 207 }.AsImmutable(), new BytesCount(4096L));

	private static readonly MagicBytes MachO32Reverse = new MagicBytes(new byte[4] { 206, 250, 237, 254 }.AsImmutable(), new BytesCount(0L));

	private static readonly MagicBytes MachO64Reverse = new MagicBytes(new byte[4] { 207, 250, 237, 254 }.AsImmutable(), new BytesCount(0L));

	private static readonly MagicBytes MachOCafeBabe = new MagicBytes(new byte[4] { 202, 254, 186, 190 }.AsImmutable(), new BytesCount(0L));

	private static readonly MagicBytes[] All = new MagicBytes[5] { MachO32, MachO64, MachO32Reverse, MachO64Reverse, MachOCafeBabe };

	public DoesFileMagicBytesMatchDelegate DoesFileMagicBytesMatch { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(Path path)
	{
		MagicBytes[] all = All;
		foreach (MagicBytes magicBytes in all)
		{
			if (DoesFileMagicBytesMatch(path, magicBytes, null, null))
			{
				return true;
			}
		}
		return false;
	}

	public IsFileOSXExecutable(DoesFileMagicBytesMatchDelegate doesFileMagicBytesMatch, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesFileMagicBytesMatch);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesFileMagicBytesMatch = doesFileMagicBytesMatch;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing IsFileOSXExecutable...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsFileOSXExecutable finished: success.");
			return result;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileOSXExecutable finished: UnauthorizedAccess.");
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
			LogInfo("IsFileOSXExecutable finished: NotExistingFile.");
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
			LogInfo("IsFileOSXExecutable has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileOSXExecutable failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileOSXExecutable finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsFileOSXExecutableDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsFileOSXExecutable(context.Resolve<DoesFileMagicBytesMatchDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
