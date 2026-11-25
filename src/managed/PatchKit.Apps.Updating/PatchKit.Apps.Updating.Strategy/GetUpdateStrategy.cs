using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Strategy;

public class GetUpdateStrategy
{
	public GetDirsToCreateDelegate GetDirsToCreate { get; }

	public GetEntriesToDeleteDelegate GetEntriesToDelete { get; }

	public GetFilesUpdateStrategyDelegate GetFilesUpdateStrategy { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private UpdateStrategy Execute(UpdateAppContext context, AppValidationMode validationMode, CancellationToken cancellationToken)
	{
		ImmutableArray<FileUpdateStrategy> fileStrategies = GetFilesUpdateStrategy(context, validationMode, cancellationToken, null, null, null, null, null, null);
		return new UpdateStrategy(GetDirsToCreate(context, cancellationToken, null), fileStrategies, GetEntriesToDelete(context, cancellationToken, null));
	}

	public GetUpdateStrategy(GetDirsToCreateDelegate getDirsToCreate, GetEntriesToDeleteDelegate getEntriesToDelete, GetFilesUpdateStrategyDelegate getFilesUpdateStrategy, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getDirsToCreate);
		Assert.IsNotNull(getEntriesToDelete);
		Assert.IsNotNull(getFilesUpdateStrategy);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetDirsToCreate = getDirsToCreate;
		GetEntriesToDelete = getEntriesToDelete;
		GetFilesUpdateStrategy = getFilesUpdateStrategy;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private UpdateStrategy DelegateExecute(UpdateAppContext context, AppValidationMode validationMode, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsValid(context, "context");
		LogInfo("Executing GetUpdateStrategy...");
		UpdateAppContext updateAppContext = context;
		LogTrace("context = " + updateAppContext.ToString());
		LogTrace("validationMode = " + validationMode);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			UpdateStrategy updateStrategy = Execute(context, validationMode, cancellationToken);
			EndLogRegion(enabled: true);
			UpdateStrategy updateStrategy2 = updateStrategy;
			LogTrace("result = " + updateStrategy2.ToString());
			LogInfo("GetUpdateStrategy finished: success.");
			return updateStrategy;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateStrategy finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (UpdateStrategy)obj;
		}
		catch (AppRemoteNotFound e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateStrategy finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj2 = onAppRemoteNotFound(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (UpdateStrategy)obj2;
		}
		catch (AppRemoteVersionNotFound e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateStrategy finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj3 = onAppRemoteVersionNotFound(e3);
			if (obj3 == null)
			{
				return default;
			}
			return (UpdateStrategy)obj3;
		}
		catch (InvalidAppPassword e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateStrategy finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj4 = onInvalidAppPassword(e4);
			if (obj4 == null)
			{
				return default;
			}
			return (UpdateStrategy)obj4;
		}
		catch (InvalidAppAuthKey e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateStrategy finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj5 = onInvalidAppAuthKey(e5);
			if (obj5 == null)
			{
				return default;
			}
			return (UpdateStrategy)obj5;
		}
		catch (ExpiredAppAuthKey e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateStrategy finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj6 = onExpiredAppAuthKey(e6);
			if (obj6 == null)
			{
				return default;
			}
			return (UpdateStrategy)obj6;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetUpdateStrategy has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetUpdateStrategy failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetUpdateStrategy finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetUpdateStrategyDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetUpdateStrategy(context.Resolve<GetDirsToCreateDelegate>(), context.Resolve<GetEntriesToDeleteDelegate>(), context.Resolve<GetFilesUpdateStrategyDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
