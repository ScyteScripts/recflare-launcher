using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Librsync;

namespace PatchKit.Apps;

public class PatchAppFile
{
	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public SetAppMetaDataEntryDelegate SetAppMetaDataEntry { get; }

	public PatchFileOverwriteDelegate PatchFileOverwrite { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private void Execute(App app, AppPath path, Path patchFilePath, AppVersionPatchId versionPatchId)
	{
		AppEntryInfo? appEntryInfo = GetAppEntryInfo(app, path, null);
		if (!appEntryInfo.HasValue || appEntryInfo.Value.Type == AppEntryType.Directory)
		{
			throw new NotExistingAppFile(app, path);
		}
		if ((int)appEntryInfo.Value.VersionId != (int)versionPatchId.SourceVersionId)
		{
		}
		Path path2 = GetAppEntryFullPath(app, path);
		PatchFileOverwrite(path2, patchFilePath, null, (FilePatchingFailure e) =>
		{
			throw new AppFilePatchingFailure(app, path, patchFilePath, e);
		}, (NotExistingFile e) =>
		{
			throw new UnpredictedException(e);
		}, null);
		SetAppMetaDataEntry(app, path, versionPatchId.TargetVersionId, null);
	}

	public PatchAppFile(GetAppEntryInfoDelegate getAppEntryInfo, GetAppEntryFullPathDelegate getAppEntryFullPath, SetAppMetaDataEntryDelegate setAppMetaDataEntry, PatchFileOverwriteDelegate patchFileOverwrite, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(setAppMetaDataEntry);
		Assert.IsNotNull(patchFileOverwrite);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryInfo = getAppEntryInfo;
		GetAppEntryFullPath = getAppEntryFullPath;
		SetAppMetaDataEntry = setAppMetaDataEntry;
		PatchFileOverwrite = patchFileOverwrite;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private object DelegateExecute(App app, AppPath path, Path patchFilePath, AppVersionPatchId versionPatchId, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingAppFile> onNotExistingAppFile, HandleExceptionDelegate<NotExistingDiffFile> onNotExistingDiffFile, HandleExceptionDelegate<AppFilePatchingFailure> onAppFilePatchingFailure)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		Arg.IsValid(patchFilePath, "patchFilePath");
		Arg.IsValid(versionPatchId, "versionPatchId");
		LogInfo("Executing PatchAppFile...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		LogTrace("patchFilePath = " + patchFilePath);
		AppVersionPatchId appVersionPatchId = versionPatchId;
		LogTrace("versionPatchId = " + appVersionPatchId.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Execute(app, path, patchFilePath, versionPatchId);
			EndLogRegion(enabled: true);
			LogInfo("PatchAppFile finished: success.");
			return null;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchAppFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			onUnauthorizedAccess(e);
			return null;
		}
		catch (NotExistingAppFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchAppFile finished: NotExistingAppFile.");
			if (onNotExistingAppFile == null)
			{
				throw;
			}
			onNotExistingAppFile(e2);
			return null;
		}
		catch (NotExistingDiffFile e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchAppFile finished: NotExistingDiffFile.");
			if (onNotExistingDiffFile == null)
			{
				throw;
			}
			onNotExistingDiffFile(e3);
			return null;
		}
		catch (AppFilePatchingFailure e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchAppFile finished: AppFilePatchingFailure.");
			if (onAppFilePatchingFailure == null)
			{
				throw;
			}
			onAppFilePatchingFailure(e4);
			return null;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("PatchAppFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("PatchAppFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("PatchAppFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public PatchAppFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new PatchAppFile(context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<SetAppMetaDataEntryDelegate>(), context.Resolve<PatchFileOverwriteDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
