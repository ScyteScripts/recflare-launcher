using System;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class SerializeMetaDataTo2_0_0
{
	private class SerializationData
	{
		public class Entry
		{
			[JsonProperty("path")]
			public string Path;

			[JsonProperty("versionId")]
			public int VersionId;
		}

		[JsonProperty("version")]
		public string Version;

		[JsonProperty("licenseKey")]
		public string LicenseKey;

		[JsonProperty("encryptedLicenseSecret")]
		public string EncryptedLicenseSecret;

		[JsonProperty("hashedSecret")]
		public string HashedSecret;

		[JsonProperty("installedVersionId")]
		public int? InstalledVersionId;

		[JsonProperty("entries")]
		public Entry[] Entries;
	}

	private const string FormatVersion = "2.0.0";

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Text Execute(AppMetaData value)
	{
		SerializationData value2 = new SerializationData
		{
			Version = "2.0.0",
			LicenseKey = value.Info.LicenseKey?.Value,
			EncryptedLicenseSecret = value.Info.EncryptedLicenseSecret?.Value,
			HashedSecret = value.Info.HashedSecret?.Value,
			Entries = value.Entries.Select((AppMetaDataEntry e) => new SerializationData.Entry
			{
				Path = e.Path,
				VersionId = e.VersionId.Value
			}).ToArray()
		};
		string v = JsonConvert.SerializeObject(value2, Formatting.Indented);
		Assert.IsNotNull(v);
		return new Text(v);
	}

	public SerializeMetaDataTo2_0_0(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Text DelegateExecute(AppMetaData value)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing SerializeMetaDataTo2_0_0...");
		AppMetaData appMetaData = value;
		LogTrace("value = " + appMetaData.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Text text = Execute(value);
			EndLogRegion(enabled: true);
			LogTrace("result = " + text);
			LogInfo("SerializeMetaDataTo2_0_0 finished: success.");
			return text;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("SerializeMetaDataTo2_0_0 has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SerializeMetaDataTo2_0_0 failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SerializeMetaDataTo2_0_0 finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SerializeMetaDataTo2_0_0Delegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SerializeMetaDataTo2_0_0(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
