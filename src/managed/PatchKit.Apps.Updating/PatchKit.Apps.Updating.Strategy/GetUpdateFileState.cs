using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Updating.Strategy;

public class GetUpdateFileState
{
	public GetAppMetaDataEntryDelegate GetAppMetaDataEntry { get; }

	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetAppFileSizeDelegate GetAppFileSize { get; }

	public GetAppFilexxHashDelegate GetAppFilexxHash { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private UpdateFileState? Execute(App app, AppPath path, AppValidationMode validationMode)
	{
		if (validationMode == AppValidationMode.MetaData)
		{
			AppMetaDataEntry? appMetaDataEntry = GetAppMetaDataEntry(app, path, null);
			if (!appMetaDataEntry.HasValue)
			{
				return null;
			}
			return new UpdateFileState(path, appMetaDataEntry.Value.VersionId, null, null);
		}
		AppEntryInfo? appEntryInfo = GetAppEntryInfo(app, path, null);
		if (!appEntryInfo.HasValue || appEntryInfo.Value.Type != AppEntryType.File)
		{
			return null;
		}
		AppVersionId versionId = appEntryInfo.Value.VersionId;
		BytesCount value = GetAppFileSize(app, path, null, (NotExistingAppFile e) =>
		{
			throw new UnpredictedException(e);
		});
		Hash? hash = null;
		if (validationMode == AppValidationMode.Hashes)
		{
			hash = GetAppFilexxHash(app, path, null, (NotExistingAppFile e) =>
			{
				throw new UnpredictedException(e);
			});
		}
		return new UpdateFileState(path, versionId, value, hash);
	}

	public GetUpdateFileState(GetAppMetaDataEntryDelegate getAppMetaDataEntry, GetAppEntryInfoDelegate getAppEntryInfo, GetAppFileSizeDelegate getAppFileSize, GetAppFilexxHashDelegate getAppFilexxHash, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppMetaDataEntry);
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getAppFileSize);
		Assert.IsNotNull(getAppFilexxHash);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppMetaDataEntry = getAppMetaDataEntry;
		GetAppEntryInfo = getAppEntryInfo;
		GetAppFileSize = getAppFileSize;
		GetAppFilexxHash = getAppFilexxHash;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private UpdateFileState? DelegateExecute(App app, AppPath path, AppValidationMode validationMode, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetUpdateFileState...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		LogTrace("validationMode = " + validationMode);
		try
		{
			BeginLogRegion(enabled: true);
			UpdateFileState? updateFileState = Execute(app, path, validationMode);
			EndLogRegion(enabled: true);
			UpdateFileState? updateFileState2 = updateFileState;
			LogTrace("result = " + updateFileState2.ToString());
			LogInfo("GetUpdateFileState finished: success.");
			return updateFileState;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateFileState finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (UpdateFileState?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateFileState has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetUpdateFileState failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetUpdateFileState finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetUpdateFileStateDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetUpdateFileState(context.Resolve<GetAppMetaDataEntryDelegate>(), context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetAppFileSizeDelegate>(), context.Resolve<GetAppFilexxHashDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
