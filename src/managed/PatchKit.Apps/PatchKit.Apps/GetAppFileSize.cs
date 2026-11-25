using System;
using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class GetAppFileSize
{
	public GetAppEntryFullPathDelegate GetAppEntryFullPath { get; }

	public GetAppEntryInfoDelegate GetAppEntryInfo { get; }

	public GetFileSizeDelegate GetFileSize { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private BytesCount Execute(App app, AppPath path)
	{
		if (!GetAppEntryInfo(app, path, null).HasValue)
		{
			throw new NotExistingAppFile(app, path);
		}
		Path path2 = GetAppEntryFullPath(app, path);
		return GetFileSize(path2, null, (NotExistingFile e) =>
		{
			throw new UnpredictedException(e);
		});
	}

	public GetAppFileSize(GetAppEntryFullPathDelegate getAppEntryFullPath, GetAppEntryInfoDelegate getAppEntryInfo, GetFileSizeDelegate getFileSize, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppEntryFullPath);
		Assert.IsNotNull(getAppEntryInfo);
		Assert.IsNotNull(getFileSize);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppEntryFullPath = getAppEntryFullPath;
		GetAppEntryInfo = getAppEntryInfo;
		GetFileSize = getFileSize;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private BytesCount DelegateExecute(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingAppFile> onNotExistingAppFile)
	{
		Arg.IsNotNull(app, "app");
		Arg.IsValid(path, "path");
		LogInfo("Executing GetAppFileSize...");
		LogTrace("app = " + app);
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			BytesCount bytesCount = Execute(app, path);
			EndLogRegion(enabled: true);
			BytesCount bytesCount2 = bytesCount;
			LogTrace("result = " + bytesCount2.ToString());
			LogInfo("GetAppFileSize finished: success.");
			return bytesCount;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppFileSize finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (BytesCount)obj;
		}
		catch (NotExistingAppFile e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppFileSize finished: NotExistingAppFile.");
			if (onNotExistingAppFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingAppFile(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (BytesCount)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetAppFileSize has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppFileSize failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetAppFileSize finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppFileSizeDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppFileSize(context.Resolve<GetAppEntryFullPathDelegate>(), context.Resolve<GetAppEntryInfoDelegate>(), context.Resolve<GetFileSizeDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
