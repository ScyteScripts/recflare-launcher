using System;
using System.Diagnostics;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class AddFileExecutableRightsIfRequired
{
	private static readonly Path ChmodPath = new Path("/bin/chmod");

	public IsFileUnixExecutableDelegate IsFileUnixExecutable { get; }

	public GetCurrentPlatformTypeDelegate GetCurrentPlatformType { get; }

	public DoesFileExistDelegate DoesFileExist { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(Path path)
	{
		PlatformType platformType = GetCurrentPlatformType();
		if (platformType == PlatformType.Linux64 || platformType == PlatformType.Linux32 || platformType == PlatformType.Osx64)
		{
			bool v = DoesFileExist(ChmodPath, (UnauthorizedAccess e) => new UnpredictedCondition("/bin/chmod should be accessible"));
			Assert.IsTrue(v, "doesBinChmodExist");
			if (IsFileUnixExecutable(path, null, null))
			{
				Process process = new Process
				{
					StartInfo = 
					{
						FileName = ChmodPath,
						Arguments = $"+x \"{path}\"",
						UseShellExecute = false
					}
				};
				process.Start();
				process.WaitForExit();
			}
		}
	}

	public AddFileExecutableRightsIfRequired(IsFileUnixExecutableDelegate isFileUnixExecutable, GetCurrentPlatformTypeDelegate getCurrentPlatformType, DoesFileExistDelegate doesFileExist, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(isFileUnixExecutable);
		Assert.IsNotNull(getCurrentPlatformType);
		Assert.IsNotNull(doesFileExist);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		IsFileUnixExecutable = isFileUnixExecutable;
		GetCurrentPlatformType = getCurrentPlatformType;
		DoesFileExist = doesFileExist;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing AddFileExecutableRightsIfRequired...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Execute(path);
			EndLogRegion(enabled: true);
			LogInfo("AddFileExecutableRightsIfRequired finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("AddFileExecutableRightsIfRequired finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("AddFileExecutableRightsIfRequired finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			onNotExistingFile(e2);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("AddFileExecutableRightsIfRequired has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("AddFileExecutableRightsIfRequired failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("AddFileExecutableRightsIfRequired finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public AddFileExecutableRightsIfRequiredDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new AddFileExecutableRightsIfRequired(context.Resolve<IsFileUnixExecutableDelegate>(), context.Resolve<GetCurrentPlatformTypeDelegate>(), context.Resolve<DoesFileExistDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
