using System;
using Autofac;
using PatchKit.Apps.Serialization;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class ReadAppExtraEntriesInfo
{
	public GetAppExtraEntriesInfoFilePathDelegate GetAppExtraEntriesInfoFilePath { get; }

	public ReadTextFromFileDelegate ReadTextFromFile { get; }

	public DeserializeExtraEntriesInfoDelegate DeserializeExtraEntriesInfo { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppExtraEntryInfo> Execute(App app)
	{
		Path path = GetAppExtraEntriesInfoFilePath(app);
		Text value = ReadTextFromFile(path, null, (NotExistingFile e) => new Text("[]"));
		return DeserializeExtraEntriesInfo(value, (MetaDataDeserializeFailure e) => ImmutableArray<AppExtraEntryInfo>.Empty);
	}

	public ReadAppExtraEntriesInfo(GetAppExtraEntriesInfoFilePathDelegate getAppExtraEntriesInfoFilePath, ReadTextFromFileDelegate readTextFromFile, DeserializeExtraEntriesInfoDelegate deserializeExtraEntriesInfo, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getAppExtraEntriesInfoFilePath);
		Assert.IsNotNull(readTextFromFile);
		Assert.IsNotNull(deserializeExtraEntriesInfo);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetAppExtraEntriesInfoFilePath = getAppExtraEntriesInfoFilePath;
		ReadTextFromFile = readTextFromFile;
		DeserializeExtraEntriesInfo = deserializeExtraEntriesInfo;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppExtraEntryInfo> DelegateExecute(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsNotNull(app, "app");
		LogInfo("Executing ReadAppExtraEntriesInfo...");
		LogTrace("app = " + app);
		try
		{
			BeginLogRegion(enabled: false);
			ImmutableArray<AppExtraEntryInfo> immutableArray = Execute(app);
			EndLogRegion(enabled: false);
			ImmutableArray<AppExtraEntryInfo> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("ReadAppExtraEntriesInfo finished: success.");
			return immutableArray;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppExtraEntriesInfo finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (ImmutableArray<AppExtraEntryInfo>)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppExtraEntriesInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("ReadAppExtraEntriesInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("ReadAppExtraEntriesInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ReadAppExtraEntriesInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ReadAppExtraEntriesInfo(context.Resolve<GetAppExtraEntriesInfoFilePathDelegate>(), context.Resolve<ReadTextFromFileDelegate>(), context.Resolve<DeserializeExtraEntriesInfoDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
