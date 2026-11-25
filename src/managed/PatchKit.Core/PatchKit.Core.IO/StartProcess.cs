using System;
using System.Diagnostics;
using System.IO;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class StartProcess
{
	public GetCurrentPlatformTypeDelegate GetCurrentPlatformType { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ProcessId Execute(Path executablePath, Text? executableArgs, Path workingDirPath)
	{
		PlatformType platformType = GetCurrentPlatformType();
		ProcessStartInfo startInfo;
		switch (platformType)
		{
		case PlatformType.Win64:
		case PlatformType.Win32:
			if (!new FileInfo(executablePath).Exists)
			{
				throw new NotExistingFile(executablePath);
			}
			startInfo = new ProcessStartInfo
			{
				FileName = executablePath,
				Arguments = (executableArgs?.Value ?? string.Empty),
				WorkingDirectory = workingDirPath,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			};
			break;
		case PlatformType.Osx64:
		{
			string text = $"-W \"{executablePath}\"";
			if (executableArgs.HasValue)
			{
				text += $" --args {executableArgs.Value}";
			}
			startInfo = new ProcessStartInfo
			{
				FileName = "open",
				Arguments = text,
				WorkingDirectory = workingDirPath,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			};
			break;
		}
		case PlatformType.Linux64:
		case PlatformType.Linux32:
			if (!new FileInfo(executablePath).Exists)
			{
				throw new NotExistingFile(executablePath);
			}
			startInfo = new ProcessStartInfo
			{
				FileName = executablePath,
				Arguments = (executableArgs?.Value ?? string.Empty),
				WorkingDirectory = workingDirPath,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			};
			break;
		default:
			throw new UnpredictedCondition($"{platformType}");
		}
		Process process = Process.Start(startInfo);
		Assert.IsNotNull(process);
		bool hasExited = process.HasExited;
		Assert.IsFalse(hasExited, "hasExited");
		process.OutputDataReceived += (object p, DataReceivedEventArgs args) =>
		{
			if (args != null && !string.IsNullOrEmpty(args.Data))
			{
				Console.WriteLine(args.Data);
			}
		};
		process.ErrorDataReceived += (object p, DataReceivedEventArgs args) =>
		{
			if (args != null && !string.IsNullOrEmpty(args.Data))
			{
				Console.WriteLine(args.Data);
			}
		};
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		return new ProcessId(process.Id);
	}

	public StartProcess(GetCurrentPlatformTypeDelegate getCurrentPlatformType, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getCurrentPlatformType);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetCurrentPlatformType = getCurrentPlatformType;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ProcessId DelegateExecute(Path executablePath, Text? executableArgs, Path workingDirPath, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(executablePath, "executablePath");
		Arg.IsValid(executableArgs, "executableArgs");
		Arg.IsValid(workingDirPath, "workingDirPath");
		LogInfo("Executing StartProcess...");
		LogTrace("executablePath = " + executablePath);
		Text? text = executableArgs;
		LogTrace("executableArgs = " + (text.HasValue ? ((string)text.GetValueOrDefault()) : null));
		LogTrace("workingDirPath = " + workingDirPath);
		try
		{
			BeginLogRegion(enabled: true);
			ProcessId processId = Execute(executablePath, executableArgs, workingDirPath);
			EndLogRegion(enabled: true);
			ProcessId processId2 = processId;
			LogTrace("result = " + processId2.ToString());
			LogInfo("StartProcess finished: success.");
			return processId;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartProcess finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (ProcessId)obj;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartProcess finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingFile(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (ProcessId)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("StartProcess has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("StartProcess failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("StartProcess finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public StartProcessDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new StartProcess(context.Resolve<GetCurrentPlatformTypeDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
