using System;
using System.Linq;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppMetaDataDeprecatedFilePaths
{
	public CombinePathDelegate CombinePath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<Path> Execute(App app)
	{
		return AppMetaDataFilePaths.Deprecated.Select((string name) => CombinePath(app.Path, new Path(name))).AsImmutable();
	}

	public GetAppMetaDataDeprecatedFilePaths(CombinePathDelegate combinePath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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

	private ImmutableArray<Path> DelegateExecute(App app)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing GetAppMetaDataDeprecatedFilePaths...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: false);
			ImmutableArray<Path> immutableArray = Execute(app);
			EndLogRegion(enabled: false);
			ImmutableArray<Path> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("GetAppMetaDataDeprecatedFilePaths finished: success.");
			return immutableArray;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppMetaDataDeprecatedFilePaths has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppMetaDataDeprecatedFilePaths failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppMetaDataDeprecatedFilePaths finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppMetaDataDeprecatedFilePathsDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppMetaDataDeprecatedFilePaths(context.Resolve<CombinePathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
