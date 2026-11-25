using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Strategy;

public class GetFileUpdateStrategy
{
	public GetUpdateFileStateDelegate GetUpdateFileState { get; }

	public IsUpdateFileConsistentDelegate IsUpdateFileConsistent { get; }

	public IsFileUpdatePatchesStrategyPossibleDelegate IsFileUpdatePatchesStrategyPossible { get; }

	public GetAppRemoteVersionFileInfoDelegate GetAppRemoteVersionFileInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private FileUpdateStrategy? Execute(UpdateAppContext context, AppValidationMode validationMode, AppPath path, CancellationToken cancellationToken)
	{
		UpdateFileState? updateFileState = GetUpdateFileState(context.App, path, validationMode, null);
		GetAppRemoteVersionFileInfoDelegate getAppRemoteVersionFileInfo = GetAppRemoteVersionFileInfo;
		AppRemote appRemote = context.AppRemote;
		AppPath path2 = path;
		BytesCount size = getAppRemoteVersionFileInfo(appRemote, context.TargetVersionId, path2, cancellationToken, null).Value.Size;
		if (!updateFileState.HasValue)
		{
			return new FileUpdateDataStrategy(path, size);
		}
		if ((int)updateFileState.Value.VersionId != (int)context.TargetVersionId)
		{
			if (IsFileUpdatePatchesStrategyPossible(context.AppRemote, context.TargetVersionId, context.LicenseSecret, context.Password, updateFileState.Value, cancellationToken, null, null, (AppRemoteVersionPatchLinkNotFound _) => false, null, null, null))
			{
				BytesCount bytesCount = size;
				AppVersionId appVersionId = new AppVersionId((int)updateFileState.Value.VersionId + 1);
				while ((int)appVersionId < (int)context.TargetVersionId)
				{
					GetAppRemoteVersionFileInfoDelegate getAppRemoteVersionFileInfo2 = GetAppRemoteVersionFileInfo;
					AppRemote appRemote2 = context.AppRemote;
					path2 = path;
					bytesCount = new BytesCount(bytesCount + getAppRemoteVersionFileInfo2(appRemote2, appVersionId, path2, cancellationToken, null).Value.Size);
					appVersionId = new AppVersionId((int)appVersionId + 1);
				}
				return new FileUpdatePatchesStrategy(path, updateFileState.Value.VersionId, bytesCount);
			}
			return new FileUpdateDataStrategy(path, size);
		}
		if (!IsUpdateFileConsistent(context.AppRemote, updateFileState.Value, cancellationToken, null))
		{
			return new FileUpdateDataStrategy(path, size);
		}
		return null;
	}

	public GetFileUpdateStrategy(GetUpdateFileStateDelegate getUpdateFileState, IsUpdateFileConsistentDelegate isUpdateFileConsistent, IsFileUpdatePatchesStrategyPossibleDelegate isFileUpdatePatchesStrategyPossible, GetAppRemoteVersionFileInfoDelegate getAppRemoteVersionFileInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getUpdateFileState);
		Assert.IsNotNull(isUpdateFileConsistent);
		Assert.IsNotNull(isFileUpdatePatchesStrategyPossible);
		Assert.IsNotNull(getAppRemoteVersionFileInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetUpdateFileState = getUpdateFileState;
		IsUpdateFileConsistent = isUpdateFileConsistent;
		IsFileUpdatePatchesStrategyPossible = isFileUpdatePatchesStrategyPossible;
		GetAppRemoteVersionFileInfo = getAppRemoteVersionFileInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private FileUpdateStrategy? DelegateExecute(UpdateAppContext context, AppValidationMode validationMode, AppPath path, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsValid(context, "context");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetFileUpdateStrategy...");
		UpdateAppContext updateAppContext = context;
		LogTrace("context = " + updateAppContext.ToString());
		LogTrace("validationMode = " + validationMode);
		LogTrace("path = " + path);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: false);
			FileUpdateStrategy? fileUpdateStrategy = Execute(context, validationMode, path, cancellationToken);
			EndLogRegion(enabled: false);
			FileUpdateStrategy? fileUpdateStrategy2 = fileUpdateStrategy;
			LogTrace("result = " + fileUpdateStrategy2.ToString());
			LogInfo("GetFileUpdateStrategy finished: success.");
			return fileUpdateStrategy;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetFileUpdateStrategy finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (FileUpdateStrategy?)obj;
		}
		catch (AppRemoteNotFound e2)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetFileUpdateStrategy finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj2 = onAppRemoteNotFound(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (FileUpdateStrategy?)obj2;
		}
		catch (AppRemoteVersionNotFound e3)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetFileUpdateStrategy finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj3 = onAppRemoteVersionNotFound(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (FileUpdateStrategy?)obj3;
		}
		catch (InvalidAppPassword e4)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetFileUpdateStrategy finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj4 = onInvalidAppPassword(e4);
			if (obj4 == null)
			{
				return null;
			}
			return (FileUpdateStrategy?)obj4;
		}
		catch (InvalidAppAuthKey e5)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetFileUpdateStrategy finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj5 = onInvalidAppAuthKey(e5);
			if (obj5 == null)
			{
				return null;
			}
			return (FileUpdateStrategy?)obj5;
		}
		catch (ExpiredAppAuthKey e6)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetFileUpdateStrategy finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj6 = onExpiredAppAuthKey(e6);
			if (obj6 == null)
			{
				return null;
			}
			return (FileUpdateStrategy?)obj6;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetFileUpdateStrategy has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetFileUpdateStrategy failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetFileUpdateStrategy finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFileUpdateStrategyDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFileUpdateStrategy(context.Resolve<GetUpdateFileStateDelegate>(), context.Resolve<IsUpdateFileConsistentDelegate>(), context.Resolve<IsFileUpdatePatchesStrategyPossibleDelegate>(), context.Resolve<GetAppRemoteVersionFileInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
