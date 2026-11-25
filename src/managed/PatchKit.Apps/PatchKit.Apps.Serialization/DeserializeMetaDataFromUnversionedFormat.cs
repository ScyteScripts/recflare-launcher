using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class DeserializeMetaDataFromUnversionedFormat
{
	private class SerializationData
	{
		[JsonProperty("_fileVersions")]
		public Dictionary<string, int> FileVersionIds;
	}

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaData Execute(Text value)
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
		if (serializationData == null)
		{
			throw new MetaDataDeserializeFailure(value);
		}
		ImmutableArray<AppMetaDataEntry> immutableArray = ((serializationData.FileVersionIds != null) ? (from x in serializationData.FileVersionIds.Select(CreateAppMetaDataEntry)
			where x.HasValue
			select x.Value).AsImmutable() : ImmutableArray<AppMetaDataEntry>.Empty);
		AppVersionId firstEntryVersionId = immutableArray[0].VersionId;
		AppVersionId? appVersionId = ((!immutableArray.All((AppMetaDataEntry x) => (int)x.VersionId == (int)firstEntryVersionId)) ? ((AppVersionId?)null) : new AppVersionId?(firstEntryVersionId));
		AppInfo info = new AppInfo(null, null, null, null, appVersionId, appVersionId, null, null, null);
		return new AppMetaData(info, immutableArray);
	}

	private AppMetaDataEntry? CreateAppMetaDataEntry(KeyValuePair<string, int> x)
	{
		try
		{
			Assert.IsNotNull(x.Key);
			return new AppMetaDataEntry(new AppPath(x.Key), new AppVersionId(x.Value));
		}
		catch (ValidationFailed exception)
		{
			LogWarning("Validation of deserialized entry failed.", exception);
			return null;
		}
	}

	public DeserializeMetaDataFromUnversionedFormat(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppMetaData DelegateExecute(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing DeserializeMetaDataFromUnversionedFormat...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: true);
			AppMetaData appMetaData = Execute(value);
			EndLogRegion(enabled: true);
			AppMetaData appMetaData2 = appMetaData;
			LogTrace("result = " + appMetaData2.ToString());
			LogInfo("DeserializeMetaDataFromUnversionedFormat finished: success.");
			return appMetaData;
		}
		catch (MetaDataDeserializeFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaDataFromUnversionedFormat finished: MetaDataDeserializeFailure.");
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
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaDataFromUnversionedFormat has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFromUnversionedFormat failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFromUnversionedFormat finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeserializeMetaDataFromUnversionedFormatDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeserializeMetaDataFromUnversionedFormat(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
