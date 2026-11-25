using System;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class DeserializeMetaData
{
	public DeserializeMetaDataFromUnversionedFormatDelegate DeserializeMetaDataFromUnversionedFormat { get; }

	public DeserializeMetaDataFrom1_x_xDelegate DeserializeMetaDataFrom1_x_x { get; }

	public DeserializeMetaDataFrom2_x_xDelegate DeserializeMetaDataFrom2_x_x { get; }

	public DeserializeMetaDataFormatVersionDelegate DeserializeMetaDataFormatVersion { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaData Execute(Text value)
	{
		MetaDataFormatVersion? metaDataFormatVersion = DeserializeMetaDataFormatVersion(value, null);
		if (!metaDataFormatVersion.HasValue)
		{
			return DeserializeMetaDataFromUnversionedFormat(value, null);
		}
		if (metaDataFormatVersion.Value.Major == 1)
		{
			return DeserializeMetaDataFrom1_x_x(value, null);
		}
		if (metaDataFormatVersion.Value.Major == 2)
		{
			return DeserializeMetaDataFrom2_x_x(value, null);
		}
		throw new UnsupportedMetaDataFormatVersion(metaDataFormatVersion.Value);
	}

	public DeserializeMetaData(DeserializeMetaDataFromUnversionedFormatDelegate deserializeMetaDataFromUnversionedFormat, DeserializeMetaDataFrom1_x_xDelegate deserializeMetaDataFrom1_x_x, DeserializeMetaDataFrom2_x_xDelegate deserializeMetaDataFrom2_x_x, DeserializeMetaDataFormatVersionDelegate deserializeMetaDataFormatVersion, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(deserializeMetaDataFromUnversionedFormat);
		Assert.IsNotNull(deserializeMetaDataFrom1_x_x);
		Assert.IsNotNull(deserializeMetaDataFrom2_x_x);
		Assert.IsNotNull(deserializeMetaDataFormatVersion);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DeserializeMetaDataFromUnversionedFormat = deserializeMetaDataFromUnversionedFormat;
		DeserializeMetaDataFrom1_x_x = deserializeMetaDataFrom1_x_x;
		DeserializeMetaDataFrom2_x_x = deserializeMetaDataFrom2_x_x;
		DeserializeMetaDataFormatVersion = deserializeMetaDataFormatVersion;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppMetaData DelegateExecute(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure, HandleExceptionAndReturnDelegate<UnsupportedMetaDataFormatVersion> onUnsupportedMetaDataFormatVersion)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing DeserializeMetaData...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: true);
			AppMetaData appMetaData = Execute(value);
			EndLogRegion(enabled: true);
			AppMetaData appMetaData2 = appMetaData;
			LogTrace("result = " + appMetaData2.ToString());
			LogInfo("DeserializeMetaData finished: success.");
			return appMetaData;
		}
		catch (MetaDataDeserializeFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaData finished: MetaDataDeserializeFailure.");
			if (onMetaDataDeserializeFailure == null)
			{
				throw;
			}
			object obj = onMetaDataDeserializeFailure(e);
			if (obj == null)
			{
				return default;
			}
			return (AppMetaData)obj;
		}
		catch (UnsupportedMetaDataFormatVersion e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaData finished: UnsupportedMetaDataFormatVersion.");
			if (onUnsupportedMetaDataFormatVersion == null)
			{
				throw;
			}
			object obj2 = onUnsupportedMetaDataFormatVersion(e2);
			if (obj2 == null)
			{
				return default;
			}
			return (AppMetaData)obj2;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaData has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaData failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaData finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeserializeMetaDataDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeserializeMetaData(context.Resolve<DeserializeMetaDataFromUnversionedFormatDelegate>(), context.Resolve<DeserializeMetaDataFrom1_x_xDelegate>(), context.Resolve<DeserializeMetaDataFrom2_x_xDelegate>(), context.Resolve<DeserializeMetaDataFormatVersionDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
