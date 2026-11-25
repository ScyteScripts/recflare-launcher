using System;
using Autofac;
using PatchKit.Apps.Serialization;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class WriteAppMetaDataCache
{
	public GetAppMetaDataFilePathDelegate GetAppMetaDataFilePath { get; }

	public SerializeMetaDataTo1_1_0Delegate SerializeMetaDataTo1_1_0 { get; }

	public DeleteFileIfExistsDelegate DeleteFileIfExists { get; }

	public WriteTextToFileDelegate WriteTextToFile { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app)
	{
		if (app.MetaDataCache.HasValue)
		{
			Path path = GetAppMetaDataFilePath(app);
			DeleteFileIfExists(path, null);
			WriteTextToFile(path, SerializeMetaDataTo1_1_0(app.MetaDataCache.Value), null, (ConflictingDir e) =>
			{
				throw new UnpredictedException(e);
			}, (ConflictingFile e) =>
			{
				throw new UnpredictedException(e);
			});
		}
	}

	public WriteAppMetaDataCache(GetAppMetaDataFilePathDelegate getAppMetaDataFilePath, SerializeMetaDataTo1_1_0Delegate serializeMetaDataTo1_1_0, DeleteFileIfExistsDelegate deleteFileIfExists, WriteTextToFileDelegate writeTextToFile, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppMetaDataFilePath);
		Assert.IsNotNull(serializeMetaDataTo1_1_0);
		Assert.IsNotNull(deleteFileIfExists);
		Assert.IsNotNull(writeTextToFile);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppMetaDataFilePath = getAppMetaDataFilePath;
		SerializeMetaDataTo1_1_0 = serializeMetaDataTo1_1_0;
		DeleteFileIfExists = deleteFileIfExists;
		WriteTextToFile = writeTextToFile;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing WriteAppMetaDataCache...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: false);
			Execute(app);
			EndLogRegion(enabled: false);
			LogInfo("WriteAppMetaDataCache finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: false);
			LogInfo("WriteAppMetaDataCache finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("WriteAppMetaDataCache has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("WriteAppMetaDataCache failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("WriteAppMetaDataCache finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public WriteAppMetaDataCacheDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new WriteAppMetaDataCache(context.Resolve<GetAppMetaDataFilePathDelegate>(), context.Resolve<SerializeMetaDataTo1_1_0Delegate>(), context.Resolve<DeleteFileIfExistsDelegate>(), context.Resolve<WriteTextToFileDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
