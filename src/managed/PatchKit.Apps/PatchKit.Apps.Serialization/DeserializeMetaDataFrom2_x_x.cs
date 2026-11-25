using System;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class DeserializeMetaDataFrom2_x_x
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

	public DeserializeMetaDataFormatVersionDelegate DeserializeMetaDataFormatVersion { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaData Execute(Text value)
	{
		MetaDataFormatVersion? metaDataFormatVersion = DeserializeMetaDataFormatVersion(value, null);
		if (!metaDataFormatVersion.HasValue || metaDataFormatVersion.Value.Major != 2)
		{
			throw new MetaDataDeserializeFailure(value);
		}
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
		AppLicenseKey? licenseKey;
		if (serializationData.LicenseKey == null)
		{
			licenseKey = null;
		}
		else
		{
			try
			{
				licenseKey = new AppLicenseKey(serializationData.LicenseKey);
			}
			catch (ValidationFailed exception)
			{
				LogWarning("Validation of deserialized license key failed.", exception);
				licenseKey = null;
			}
		}
		AppEncryptedLicenseSecret? encryptedLicenseSecret;
		if (serializationData.EncryptedLicenseSecret == null)
		{
			encryptedLicenseSecret = null;
		}
		else
		{
			try
			{
				encryptedLicenseSecret = new AppEncryptedLicenseSecret(serializationData.EncryptedLicenseSecret);
			}
			catch (ValidationFailed exception2)
			{
				LogWarning("Validation of deserialized encrypted license secret failed.", exception2);
				encryptedLicenseSecret = null;
			}
		}
		AppHashedSecret? hashedSecret;
		if (serializationData.HashedSecret == null)
		{
			hashedSecret = null;
		}
		else
		{
			try
			{
				hashedSecret = new AppHashedSecret(serializationData.HashedSecret);
			}
			catch (ValidationFailed exception3)
			{
				LogWarning("Validation of deserialized secret failed.", exception3);
				hashedSecret = null;
			}
		}
		AppVersionId? appVersionId;
		if (!serializationData.InstalledVersionId.HasValue)
		{
			appVersionId = null;
		}
		else
		{
			try
			{
				appVersionId = new AppVersionId(serializationData.InstalledVersionId.Value);
			}
			catch (ValidationFailed exception4)
			{
				LogWarning("Validation of deserialized installed version id failed.", exception4);
				appVersionId = null;
			}
		}
		ImmutableArray<AppMetaDataEntry> entries = ((serializationData.Entries != null) ? (from x in serializationData.Entries.Select(CreateAppMetaDataEntry)
			where x.HasValue
			select x.Value).AsImmutable() : ImmutableArray<AppMetaDataEntry>.Empty);
		AppInfo info = new AppInfo(hashedSecret, licenseKey, encryptedLicenseSecret, null, appVersionId, appVersionId, null, null, null);
		return new AppMetaData(info, entries);
	}

	private AppMetaDataEntry? CreateAppMetaDataEntry(SerializationData.Entry data)
	{
		if (data == null)
		{
			LogWarning("Null entry.");
			return null;
		}
		try
		{
			if (data.Path != null)
			{
				return new AppMetaDataEntry(new AppPath(data.Path), new AppVersionId(data.VersionId));
			}
			LogWarning("Null entry path.");
			return null;
		}
		catch (ValidationFailed exception)
		{
			LogWarning("Validation of deserialized entry failed.", exception);
			return null;
		}
	}

	public DeserializeMetaDataFrom2_x_x(DeserializeMetaDataFormatVersionDelegate deserializeMetaDataFormatVersion, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(deserializeMetaDataFormatVersion);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		DeserializeMetaDataFormatVersion = deserializeMetaDataFormatVersion;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppMetaData DelegateExecute(Text value, HandleExceptionAndReturnDelegate<MetaDataDeserializeFailure> onMetaDataDeserializeFailure)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing DeserializeMetaDataFrom2_x_x...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: true);
			AppMetaData appMetaData = Execute(value);
			EndLogRegion(enabled: true);
			AppMetaData appMetaData2 = appMetaData;
			LogTrace("result = " + appMetaData2.ToString());
			LogInfo("DeserializeMetaDataFrom2_x_x finished: success.");
			return appMetaData;
		}
		catch (MetaDataDeserializeFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaDataFrom2_x_x finished: MetaDataDeserializeFailure.");
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
			LogInfo("DeserializeMetaDataFrom2_x_x has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFrom2_x_x failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFrom2_x_x finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeserializeMetaDataFrom2_x_xDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeserializeMetaDataFrom2_x_x(context.Resolve<DeserializeMetaDataFormatVersionDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
