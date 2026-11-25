using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppMetaDataFilePath
{
	public CombinePathDelegate CombinePath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Path Execute(App app)
	{
		return CombinePath(app.Path, new Path(AppMetaDataFilePaths.Current));
	}

	public GetAppMetaDataFilePath(CombinePathDelegate combinePath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(combinePath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		CombinePath = combinePath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Path DelegateExecute(App app)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GetAppMetaDataFilePath...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: false);
			Path path = Execute(app);
			EndLogRegion(enabled: false);
			LogTrace("result = " + path);
			LogInfo("GetAppMetaDataFilePath finished: success.");
			return path;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppMetaDataFilePath has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppMetaDataFilePath failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppMetaDataFilePath finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppMetaDataFilePathDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppMetaDataFilePath(context.Resolve<CombinePathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
