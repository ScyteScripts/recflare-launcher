using System;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GenerateAppStartExecutableArgs
{
	public GetCurrentPlatformTypeDelegate GetCurrentPlatformType { get; }

	public CombinePathDelegate CombinePath { get; }

	public DoesFileExistDelegate DoesFileExist { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Text? Execute(App app)
	{
		PlatformType platformType = GetCurrentPlatformType();
		if (platformType == PlatformType.Win64 || platformType == PlatformType.Win32)
		{
			string text = null;
			if (DoesFileExist(CombinePath(app.Path, new Path(AppMetaDataFilePaths.Current)), null))
			{
				text = AppMetaDataFilePaths.Current;
			}
			else
			{
				foreach (string item in AppMetaDataFilePaths.Deprecated)
				{
					if (DoesFileExist(CombinePath(app.Path, new Path(item)), null))
					{
						text = item;
						break;
					}
				}
			}
			if (text != null)
			{
				return new Text("+patcher-data-location " + text);
			}
		}
		return null;
	}

	public GenerateAppStartExecutableArgs(GetCurrentPlatformTypeDelegate getCurrentPlatformType, CombinePathDelegate combinePath, DoesFileExistDelegate doesFileExist, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getCurrentPlatformType);
		Assert.IsNotNull(combinePath);
		Assert.IsNotNull(doesFileExist);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetCurrentPlatformType = getCurrentPlatformType;
		CombinePath = combinePath;
		DoesFileExist = doesFileExist;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Text? DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GenerateAppStartExecutableArgs...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: true);
			Text? text = Execute(app);
			EndLogRegion(enabled: true);
			Text? text2 = text;
			LogTrace("result = " + (text2.HasValue ? ((string)text2.GetValueOrDefault()) : null));
			LogInfo("GenerateAppStartExecutableArgs finished: success.");
			return text;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GenerateAppStartExecutableArgs finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (Text?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GenerateAppStartExecutableArgs has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GenerateAppStartExecutableArgs failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GenerateAppStartExecutableArgs finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GenerateAppStartExecutableArgsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GenerateAppStartExecutableArgs(context.Resolve<GetCurrentPlatformTypeDelegate>(), context.Resolve<CombinePathDelegate>(), context.Resolve<DoesFileExistDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
