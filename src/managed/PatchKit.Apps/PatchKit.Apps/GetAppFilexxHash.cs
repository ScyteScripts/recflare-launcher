using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps;

public class GetAppFilexxHash
{
	public DoesAppFileExistDelegate DoesAppFileExist { get; }

	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public GetFilexxHashDelegate GetFilexxHash { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Hash Execute(App app, AppPath path)
	{
		if (!DoesAppFileExist(app, path, null))
		{
			throw new NotExistingAppFile(app, path);
		}
		Path path2 = GetAppEntryFullPath(app, path);
		xxHashSeed seed = new xxHashSeed(42);
		return GetFilexxHash(path2, seed, null, (NotExistingFile e) =>
		{
			throw new UnpredictedException(e);
		});
	}

	public GetAppFilexxHash(DoesAppFileExistDelegate doesAppFileExist, GetAppEntryFullPathDelegate getAppEntryFullPath, GetFilexxHashDelegate getFilexxHash, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(doesAppFileExist);
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(getFilexxHash);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DoesAppFileExist = doesAppFileExist;
		GetAppEntryFullPath = getAppEntryFullPath;
		GetFilexxHash = getFilexxHash;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Hash DelegateExecute(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingAppFile> onNotExistingAppFile)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetAppFilexxHash...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			Hash hash = Execute(app, path);
			EndLogRegion(enabled: true);
			Hash hash2 = hash;
			LogTrace("result = " + hash2.ToString());
			LogInfo("GetAppFilexxHash finished: success.");
			return hash;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppFilexxHash finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (Hash)obj;
		}
		catch (NotExistingAppFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppFilexxHash finished: NotExistingAppFile.");
			if (onNotExistingAppFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingAppFile(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (Hash)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppFilexxHash has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppFilexxHash failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppFilexxHash finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppFilexxHashDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppFilexxHash(context.Resolve<DoesAppFileExistDelegate>(), context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<GetFilexxHashDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
