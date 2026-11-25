using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Strategy;

public class GetFilesUpdateStrategy
{
	public GetAppRemoteVersionFilePathsDelegate GetAppRemoteVersionFilePaths { get; }

	public GetFileUpdateStrategyDelegate GetFileUpdateStrategy { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<FileUpdateStrategy> Execute(UpdateAppContext context, AppValidationMode validationMode, CancellationToken cancellationToken)
	{
		ImmutableArray<AppPath> immutableArray = GetAppRemoteVersionFilePaths(context.AppRemote, context.TargetVersionId, cancellationToken, null);
		List<FileUpdateStrategy> list = new List<FileUpdateStrategy>();
		foreach (AppPath item in immutableArray)
		{
			cancellationToken.ThrowIfCancellationRequested();
			FileUpdateStrategy? fileUpdateStrategy = GetFileUpdateStrategy(context, validationMode, item, cancellationToken, null, null, null, null, null, null);
			if (fileUpdateStrategy.HasValue)
			{
				list.Add(fileUpdateStrategy.Value);
			}
		}
		return list.AsImmutable();
	}

	public GetFilesUpdateStrategy(GetAppRemoteVersionFilePathsDelegate getAppRemoteVersionFilePaths, GetFileUpdateStrategyDelegate getFileUpdateStrategy, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppRemoteVersionFilePaths);
		Assert.IsNotNull(getFileUpdateStrategy);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppRemoteVersionFilePaths = getAppRemoteVersionFilePaths;
		GetFileUpdateStrategy = getFileUpdateStrategy;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<FileUpdateStrategy> DelegateExecute(UpdateAppContext context, AppValidationMode validationMode, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsValid(context, "context");
		LogInfo("Executing GetFilesUpdateStrategy...");
		UpdateAppContext updateAppContext = context;
		LogTrace("context = " + updateAppContext.ToString());
		LogTrace("validationMode = " + validationMode);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<FileUpdateStrategy> immutableArray = Execute(context, validationMode, cancellationToken);
			EndLogRegion(enabled: true);
			ImmutableArray<FileUpdateStrategy> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("GetFilesUpdateStrategy finished: success.");
			return immutableArray;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilesUpdateStrategy finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (ImmutableArray<FileUpdateStrategy>)obj;
		}
		catch (AppRemoteNotFound e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilesUpdateStrategy finished: AppRemoteNotFound.");
			if (onAppRemoteNotFound == null)
			{
				throw;
			}
			object obj2 = onAppRemoteNotFound(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (ImmutableArray<FileUpdateStrategy>)obj2;
		}
		catch (AppRemoteVersionNotFound e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilesUpdateStrategy finished: AppRemoteVersionNotFound.");
			if (onAppRemoteVersionNotFound == null)
			{
				throw;
			}
			object obj3 = onAppRemoteVersionNotFound(e3);
			if (obj3 == null)
			{
				return default;
			}
			return (ImmutableArray<FileUpdateStrategy>)obj3;
		}
		catch (InvalidAppPassword e4)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilesUpdateStrategy finished: InvalidAppPassword.");
			if (onInvalidAppPassword == null)
			{
				throw;
			}
			object obj4 = onInvalidAppPassword(e4);
			if (obj4 == null)
			{
				return default;
			}
			return (ImmutableArray<FileUpdateStrategy>)obj4;
		}
		catch (InvalidAppAuthKey e5)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilesUpdateStrategy finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj5 = onInvalidAppAuthKey(e5);
			if (obj5 == null)
			{
				return default;
			}
			return (ImmutableArray<FileUpdateStrategy>)obj5;
		}
		catch (ExpiredAppAuthKey e6)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilesUpdateStrategy finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj6 = onExpiredAppAuthKey(e6);
			if (obj6 == null)
			{
				return default;
			}
			return (ImmutableArray<FileUpdateStrategy>)obj6;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetFilesUpdateStrategy has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilesUpdateStrategy failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetFilesUpdateStrategy finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetFilesUpdateStrategyDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetFilesUpdateStrategy(context.Resolve<GetAppRemoteVersionFilePathsDelegate>(), context.Resolve<GetFileUpdateStrategyDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
