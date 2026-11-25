using System;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Serialization;

public class DeserializeExtraEntriesInfo
{
	public struct AppExtraEntryInfoSerializationData
	{
		[JsonProperty("path")]
		public string Path;
	}

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private ImmutableArray<AppExtraEntryInfo> Execute(Text value)
	{
		AppExtraEntryInfoSerializationData[] array;
		try
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				Converters = { (JsonConverter)new StringEnumConverter(new SnakeCaseNamingStrategy()) }
			};
			array = JsonConvert.DeserializeObject<AppExtraEntryInfoSerializationData[]>(value, settings);
		}
		catch (Exception inner)
		{
			throw new MetaDataDeserializeFailure(value, inner);
		}
		if (array == null)
		{
			throw new MetaDataDeserializeFailure(value);
		}
		return array.Select((AppExtraEntryInfoSerializationData x) => new AppExtraEntryInfo(new Path(x.Path))).AsImmutable();
	}

	public DeserializeExtraEntriesInfo(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private ImmutableArray<AppExtraEntryInfo> DelegateExecute(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing DeserializeExtraEntriesInfo...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: true);
			ImmutableArray<AppExtraEntryInfo> immutableArray = Execute(value);
			EndLogRegion(enabled: true);
			ImmutableArray<AppExtraEntryInfo> immutableArray2 = immutableArray;
			LogTrace("result = " + immutableArray2.ToString());
			LogInfo("DeserializeExtraEntriesInfo finished: success.");
			return immutableArray;
		}
		catch (MetaDataDeserializeFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeExtraEntriesInfo finished: MetaDataDeserializeFailure.");
			if (onMetaDataDeserializeFailure == null)
			{
				throw;
			}
			object obj = onMetaDataDeserializeFailure(e);
			if (obj == null)
			{
				return default;
			}
			return (ImmutableArray<AppExtraEntryInfo>)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeExtraEntriesInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeExtraEntriesInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeExtraEntriesInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeserializeExtraEntriesInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeserializeExtraEntriesInfo(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
