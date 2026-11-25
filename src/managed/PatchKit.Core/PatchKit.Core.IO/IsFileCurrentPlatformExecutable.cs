using System;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class IsFileCurrentPlatformExecutable
{
	public GetCurrentPlatformTypeDelegate GetCurrentPlatformType { get; }

	public IsFileWindowsExecutableDelegate IsFileWindowsExecutable { get; }

	public IsFileOSXExecutableDelegate IsFileOSXExecutable { get; }

	public IsFileELFExecutableDelegate IsFileELFExecutable { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private bool Execute(Path path)
	{
		PlatformType platformType = GetCurrentPlatformType();
		switch (platformType)
		{
		case PlatformType.Linux64:
		case PlatformType.Linux32:
			return IsFileELFExecutable(path, null, null);
		case PlatformType.Osx64:
			return IsFileOSXExecutable(path, null, null);
		case PlatformType.Win64:
		case PlatformType.Win32:
			return IsFileWindowsExecutable(path, null, null);
		default:
			throw new UnpredictedCondition(platformType.ToString());
		}
	}

	public IsFileCurrentPlatformExecutable(GetCurrentPlatformTypeDelegate getCurrentPlatformType, IsFileWindowsExecutableDelegate isFileWindowsExecutable, IsFileOSXExecutableDelegate isFileOSXExecutable, IsFileELFExecutableDelegate isFileELFExecutable, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getCurrentPlatformType);
		Assert.IsNotNull(isFileWindowsExecutable);
		Assert.IsNotNull(isFileOSXExecutable);
		Assert.IsNotNull(isFileELFExecutable);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetCurrentPlatformType = getCurrentPlatformType;
		IsFileWindowsExecutable = isFileWindowsExecutable;
		IsFileOSXExecutable = isFileOSXExecutable;
		IsFileELFExecutable = isFileELFExecutable;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private bool DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing IsFileCurrentPlatformExecutable...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			bool result = Execute(path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + result);
			LogInfo("IsFileCurrentPlatformExecutable finished: success.");
			return result;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("IsFileCurrentPlatformExecutable finished: UnauthorizedAccess.");
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
			LogInfo("IsFileCurrentPlatformExecutable finished: NotExistingFile.");
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
			LogInfo("IsFileCurrentPlatformExecutable has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileCurrentPlatformExecutable failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("IsFileCurrentPlatformExecutable finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public IsFileCurrentPlatformExecutableDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new IsFileCurrentPlatformExecutable(context.Resolve<GetCurrentPlatformTypeDelegate>(), context.Resolve<IsFileWindowsExecutableDelegate>(), context.Resolve<IsFileOSXExecutableDelegate>(), context.Resolve<IsFileELFExecutableDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
