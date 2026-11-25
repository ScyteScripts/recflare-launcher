using System;
using Autofac;
using Newtonsoft.Json;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class DeserializeMetaDataFormatVersion
{
	private class SerializationData
	{
		[JsonProperty("version")]
		public string Version;
	}

	public ParseMetaDataFormatVersionDelegate ParseMetaDataFormatVersion { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private MetaDataFormatVersion? Execute(Text value)
	{
		SerializationData serializationData;
		try
		{
			serializationData = JsonConvert.DeserializeObject<SerializationData>(value);
		}
		catch (Exception inner)
		{
			throw new MetaDataDeserializeFailure(value, inner);
		}
		if (serializationData == null || string.IsNullOrEmpty(serializationData.Version))
		{
			return null;
		}
		return ParseMetaDataFormatVersion(new Text(serializationData.Version), (MetaDataFormatVersionParseFailure e) =>
		{
			throw new MetaDataDeserializeFailure(value, e);
		});
	}

	public DeserializeMetaDataFormatVersion(ParseMetaDataFormatVersionDelegate parseMetaDataFormatVersion, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(parseMetaDataFormatVersion);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		ParseMetaDataFormatVersion = parseMetaDataFormatVersion;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private MetaDataFormatVersion? DelegateExecute(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing DeserializeMetaDataFormatVersion...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: true);
			MetaDataFormatVersion? metaDataFormatVersion = Execute(value);
			EndLogRegion(enabled: true);
			MetaDataFormatVersion? metaDataFormatVersion2 = metaDataFormatVersion;
			LogTrace("result = " + metaDataFormatVersion2.ToString());
			LogInfo("DeserializeMetaDataFormatVersion finished: success.");
			return metaDataFormatVersion;
		}
		catch (MetaDataDeserializeFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaDataFormatVersion finished: MetaDataDeserializeFailure.");
			if (onMetaDataDeserializeFailure == null)
			{
				throw;
			}
			object obj = onMetaDataDeserializeFailure(e);
			if (obj == null)
			{
				return null;
			}
			return (MetaDataFormatVersion?)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaDataFormatVersion has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFormatVersion failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFormatVersion finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeserializeMetaDataFormatVersionDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeserializeMetaDataFormatVersion(context.Resolve<ParseMetaDataFormatVersionDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
