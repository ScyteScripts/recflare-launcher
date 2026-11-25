using System;
using System.IO;
using System.Threading;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class ReadTextFromFile
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Text Execute(Path path)
	{
		try
		{
			Text? text = null;
			do
			{
				try
				{
					text = new Text(File.ReadAllText(path));
				}
				catch (DirectoryNotFoundException inner)
				{
					throw new NotExistingFile(path, inner);
				}
				catch (FileNotFoundException inner2)
				{
					throw new NotExistingFile(path, inner2);
				}
				catch (IOException exception)
				{
					LogWarning("Retrying in 1 second...", exception);
					Thread.Sleep(1000);
				}
			}
			while (!text.HasValue);
			return text.Value;
		}
		catch (UnauthorizedAccessException inner3)
		{
			throw new UnauthorizedAccess(inner3);
		}
	}

	public ReadTextFromFile(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Text DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing ReadTextFromFile...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Text text = Execute(path);
			EndLogRegion(enabled: true);
			LogTrace("result = " + text);
			LogInfo("ReadTextFromFile finished: success.");
			return text;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("ReadTextFromFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (Text)obj;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("ReadTextFromFile finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingFile(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (Text)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("ReadTextFromFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("ReadTextFromFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("ReadTextFromFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ReadTextFromFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ReadTextFromFile(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
