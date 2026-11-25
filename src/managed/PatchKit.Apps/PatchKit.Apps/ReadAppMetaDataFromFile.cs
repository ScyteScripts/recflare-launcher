using System;
using Autofac;
using PatchKit.Apps.Serialization;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class ReadAppMetaDataFromFile
{
	public ReadTextFromFileDelegate ReadTextFromFile { get; }

	public DeserializeMetaDataDelegate DeserializeMetaData { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaData? Execute(Path path)
	{
		Text value = ReadTextFromFile(path, null, null);
		return DeserializeMetaData(value, null, null);
	}

	public ReadAppMetaDataFromFile(ReadTextFromFileDelegate readTextFromFile, DeserializeMetaDataDelegate deserializeMetaData, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(readTextFromFile);
		Assert.IsNotNull(deserializeMetaData);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ReadTextFromFile = readTextFromFile;
		DeserializeMetaData = deserializeMetaData;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppMetaData? DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure, HandleExceptionAndReturnDelegate<UnsupportedMetaDataFormatVersion> onUnsupportedMetaDataFormatVersion)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing ReadAppMetaDataFromFile...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: false);
			AppMetaData? appMetaData = Execute(path);
			EndLogRegion(enabled: false);
			AppMetaData? appMetaData2 = appMetaData;
			LogTrace("result = " + appMetaData2.ToString());
			LogInfo("ReadAppMetaDataFromFile finished: success.");
			return appMetaData;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppMetaDataFromFile finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return null;
			}
			return (AppMetaData?)obj;
		}
		catch (NotExistingFile e2)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppMetaDataFromFile finished: NotExistingFile.");
			if (onNotExistingFile == null)
			{
				throw;
			}
			object obj2 = onNotExistingFile(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (AppMetaData?)obj2;
		}
		catch (MetaDataDeserializeFailure e3)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppMetaDataFromFile finished: MetaDataDeserializeFailure.");
			if (onMetaDataDeserializeFailure == null)
			{
				throw;
			}
			object obj3 = onMetaDataDeserializeFailure(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (AppMetaData?)obj3;
		}
		catch (UnsupportedMetaDataFormatVersion e4)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppMetaDataFromFile finished: UnsupportedMetaDataFormatVersion.");
			if (onUnsupportedMetaDataFormatVersion == null)
			{
				throw;
			}
			object obj4 = onUnsupportedMetaDataFormatVersion(e4);
			if (obj4 == null)
			{
				return null;
			}
			return (AppMetaData?)obj4;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("ReadAppMetaDataFromFile has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("ReadAppMetaDataFromFile failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("ReadAppMetaDataFromFile finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ReadAppMetaDataFromFileDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ReadAppMetaDataFromFile(context.Resolve<ReadTextFromFileDelegate>(), context.Resolve<DeserializeMetaDataDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
