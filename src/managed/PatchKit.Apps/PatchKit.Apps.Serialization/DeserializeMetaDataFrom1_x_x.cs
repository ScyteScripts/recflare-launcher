using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Serialization;

public class DeserializeMetaDataFrom1_x_x
{
	public struct AppFilesOperationTaskProgressSerializationData
	{
		[JsonProperty("bytesCount")]
		public long BytesCount;

		[JsonProperty("totalBytesCount")]
		public long TotalBytesCount;

		[JsonProperty("bytesCountPerSecond")]
		public long BytesCountPerSecond;
	}

	public struct AppLastInstallTaskStateSerializationData
	{
		[JsonProperty("progress")]
		public AppFilesOperationTaskProgressSerializationData? Progress;

		[JsonProperty("error")]
		public AppInstallTaskError? Error;
	}

	public struct AppLastUpdateTaskStateSerializationData
	{
		[JsonProperty("progress")]
		public AppFilesOperationTaskProgressSerializationData? Progress;

		[JsonProperty("error")]
		public AppUpdateTaskError? Error;
	}

	public struct AppLastRepairTaskStateSerializationData
	{
		[JsonProperty("progress")]
		public AppFilesOperationTaskProgressSerializationData? Progress;

		[JsonProperty("error")]
		public AppRepairTaskError? Error;
	}

	public struct AppLastFilesOperationTaskStateSerializationData
	{
		[JsonProperty("lastInstallTaskState")]
		public AppLastInstallTaskStateSerializationData? AppLastInstallTaskState;

		[JsonProperty("lastUpdateTaskState")]
		public AppLastUpdateTaskStateSerializationData? AppLastUpdateTaskState;

		[JsonProperty("lastRepairTaskState")]
		public AppLastRepairTaskStateSerializationData? AppLastRepairTaskState;
	}

	public struct AppLastUnfinishedWriteFileDataOperationInfoSerializationData
	{
		[JsonProperty("filePath")]
		public string FilePath;

		[JsonProperty("fileVersionId")]
		public int FileVersionId;

		[JsonProperty("fileLastBytesCount")]
		public long FileLastBytesCount;
	}

	private class SerializationData
	{
		[JsonProperty("version")]
		public string Version;

		[JsonProperty("hashed_secret")]
		public string HashedSecret;

		[JsonProperty("product_key")]
		public string ProductKey;

		[JsonProperty("encrypted_license_secret")]
		public string EncryptedLicenseSecret;

		[JsonProperty("encryptedPassword")]
		public string EncryptedPassword;

		[JsonProperty("is_installed")]
		public bool? IsInstalled;

		[JsonProperty("installed_version_id")]
		public int? InstalledVersionId;

		[JsonProperty("last_installed_version_id")]
		public int? LastInstalledVersionId;

		[JsonProperty("start_exe_path")]
		public string StartExePath;

		[JsonProperty("start_exe_args")]
		public string StartExeArgs;

		[JsonProperty("lastFilesOperationTaskState")]
		public AppLastFilesOperationTaskStateSerializationData? LastFilesOperationTaskState;

		[JsonProperty("lastUnfinishedWriteFileDataOperationInfo")]
		public AppLastUnfinishedWriteFileDataOperationInfoSerializationData? LastUnfinishedWriteFileDataOperationInfo;

		[JsonProperty("_fileVersions")]
		public Dictionary<string, int> FileVersionIds;
	}

	public DeserializeMetaDataFormatVersionDelegate DeserializeMetaDataFormatVersion { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppMetaData Execute(Text value)
	{
		MetaDataFormatVersion? metaDataFormatVersion = DeserializeMetaDataFormatVersion(value, null);
		if (!metaDataFormatVersion.HasValue || metaDataFormatVersion.Value.Major != 1)
		{
			throw new MetaDataDeserializeFailure(value);
		}
		SerializationData serializationData;
		try
		{
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				Converters = { (JsonConverter)new StringEnumConverter(new SnakeCaseNamingStrategy()) }
			};
			serializationData = JsonConvert.DeserializeObject<SerializationData>(value, settings);
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
		if (serializationData.ProductKey == null || serializationData.ProductKey == string.Empty)
		{
			licenseKey = null;
		}
		else
		{
			try
			{
				licenseKey = new AppLicenseKey(serializationData.ProductKey);
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
		AppEncryptedPassword? encryptedPassword;
		if (serializationData.EncryptedPassword == null)
		{
			encryptedPassword = null;
		}
		else
		{
			try
			{
				encryptedPassword = new AppEncryptedPassword(serializationData.EncryptedPassword);
			}
			catch (ValidationFailed exception3)
			{
				LogWarning("Validation of deserialized encrypted password failed.", exception3);
				encryptedPassword = null;
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
			catch (ValidationFailed exception4)
			{
				LogWarning("Validation of deserialized secret failed.", exception4);
				hashedSecret = null;
			}
		}
		ImmutableArray<AppMetaDataEntry> immutableArray = ((serializationData.FileVersionIds != null) ? (from x in serializationData.FileVersionIds.Select(CreateAppMetaDataEntry)
			where x.HasValue
			select x.Value).AsImmutable() : ImmutableArray<AppMetaDataEntry>.Empty);
		AppVersionId? appVersionId;
		if (!serializationData.IsInstalled.HasValue)
		{
			if (immutableArray.Length > 0)
			{
				AppVersionId firstEntryVersionId = immutableArray[0].VersionId;
				appVersionId = ((!immutableArray.All((AppMetaDataEntry x) => (int)x.VersionId == (int)firstEntryVersionId)) ? ((AppVersionId?)null) : new AppVersionId?(firstEntryVersionId));
			}
			else
			{
				appVersionId = null;
			}
		}
		else if (!serializationData.IsInstalled.Value)
		{
			appVersionId = null;
		}
		else if (serializationData.InstalledVersionId.HasValue)
		{
			try
			{
				appVersionId = new AppVersionId(serializationData.InstalledVersionId.Value);
			}
			catch (ValidationFailed exception5)
			{
				LogWarning("Validation of deserialized installed version id failed.", exception5);
				appVersionId = null;
			}
		}
		else
		{
			appVersionId = null;
			LogWarning("is_installed is set to true, but installed_version_id is missing.");
		}
		AppVersionId? lastInstalledVersionId;
		if (!serializationData.LastInstalledVersionId.HasValue)
		{
			lastInstalledVersionId = appVersionId;
		}
		else
		{
			try
			{
				lastInstalledVersionId = new AppVersionId(serializationData.LastInstalledVersionId.Value);
			}
			catch (ValidationFailed exception6)
			{
				LogWarning("Validation of deserialized last installed version id failed.", exception6);
				lastInstalledVersionId = null;
			}
		}
		AppLastFilesOperationTaskState? lastFilesOperationTaskState = null;
		if (serializationData.LastFilesOperationTaskState.HasValue)
		{
			AppLastInstallTaskStateSerializationData? appLastInstallTaskState = serializationData.LastFilesOperationTaskState.Value.AppLastInstallTaskState;
			AppLastUpdateTaskStateSerializationData? appLastUpdateTaskState = serializationData.LastFilesOperationTaskState.Value.AppLastUpdateTaskState;
			AppLastRepairTaskStateSerializationData? appLastRepairTaskState = serializationData.LastFilesOperationTaskState.Value.AppLastRepairTaskState;
			if (appLastInstallTaskState.HasValue)
			{
				lastFilesOperationTaskState = new AppLastFilesOperationTaskState(new AppLastInstallTaskState(appLastInstallTaskState.Value.Progress.HasValue ? new AppFilesOperationTaskProgress?(new AppFilesOperationTaskProgress(new BytesCount(appLastInstallTaskState.Value.Progress.Value.BytesCount), new BytesCount(appLastInstallTaskState.Value.Progress.Value.TotalBytesCount), new BytesCount(appLastInstallTaskState.Value.Progress.Value.BytesCountPerSecond), new BytesCount(0L), new BytesCount(0L), new BytesCount(0L))) : ((AppFilesOperationTaskProgress?)null), appLastInstallTaskState.Value.Error), null, null);
			}
			else if (appLastUpdateTaskState.HasValue)
			{
				lastFilesOperationTaskState = new AppLastFilesOperationTaskState(null, new AppLastUpdateTaskState(appLastUpdateTaskState.Value.Progress.HasValue ? new AppFilesOperationTaskProgress?(new AppFilesOperationTaskProgress(new BytesCount(appLastUpdateTaskState.Value.Progress.Value.BytesCount), new BytesCount(appLastUpdateTaskState.Value.Progress.Value.TotalBytesCount), new BytesCount(appLastUpdateTaskState.Value.Progress.Value.BytesCountPerSecond), new BytesCount(0L), new BytesCount(0L), new BytesCount(0L))) : ((AppFilesOperationTaskProgress?)null), appLastUpdateTaskState.Value.Error), null);
			}
			else if (appLastRepairTaskState.HasValue)
			{
				lastFilesOperationTaskState = new AppLastFilesOperationTaskState(null, null, new AppLastRepairTaskState(appLastRepairTaskState.Value.Progress.HasValue ? new AppFilesOperationTaskProgress?(new AppFilesOperationTaskProgress(new BytesCount(appLastRepairTaskState.Value.Progress.Value.BytesCount), new BytesCount(appLastRepairTaskState.Value.Progress.Value.TotalBytesCount), new BytesCount(appLastRepairTaskState.Value.Progress.Value.BytesCountPerSecond), new BytesCount(0L), new BytesCount(0L), new BytesCount(0L))) : ((AppFilesOperationTaskProgress?)null), appLastRepairTaskState.Value.Error));
			}
		}
		AppLastUnfinishedWriteFileDataOperationInfo? lastUnfinishedWriteFileDataOperationInfo = null;
		if (serializationData.LastUnfinishedWriteFileDataOperationInfo.HasValue)
		{
			lastUnfinishedWriteFileDataOperationInfo = new AppLastUnfinishedWriteFileDataOperationInfo(new AppPath(serializationData.LastUnfinishedWriteFileDataOperationInfo.Value.FilePath), new AppVersionId(serializationData.LastUnfinishedWriteFileDataOperationInfo.Value.FileVersionId), new BytesCount(serializationData.LastUnfinishedWriteFileDataOperationInfo.Value.FileLastBytesCount));
		}
		AppStartInfo? appStartInfo = GetAppStartInfo(serializationData);
		AppInfo info = new AppInfo(hashedSecret, licenseKey, encryptedLicenseSecret, encryptedPassword, appVersionId, lastInstalledVersionId, appStartInfo, lastFilesOperationTaskState, lastUnfinishedWriteFileDataOperationInfo);
		return new AppMetaData(info, immutableArray);
	}

	private AppStartInfo? GetAppStartInfo(SerializationData data)
	{
		if (string.IsNullOrEmpty(data.StartExePath))
		{
			return null;
		}
		AppPath executablePath;
		try
		{
			executablePath = new AppPath(data.StartExePath);
		}
		catch (ValidationFailed exception)
		{
			LogWarning("Validation of deserialized start executable path failed.", exception);
			return null;
		}
		Text? executableArgs;
		if (data.StartExeArgs == null)
		{
			executableArgs = null;
		}
		else
		{
			try
			{
				executableArgs = new Text(data.StartExeArgs);
			}
			catch (ValidationFailed exception2)
			{
				LogWarning("Validation of deserialized start executable arguments failed.", exception2);
				executableArgs = null;
			}
		}
		return new AppStartInfo(executablePath, executableArgs);
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

	public DeserializeMetaDataFrom1_x_x(DeserializeMetaDataFormatVersionDelegate deserializeMetaDataFormatVersion, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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
		LogInfo("Executing DeserializeMetaDataFrom1_x_x...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: true);
			AppMetaData appMetaData = Execute(value);
			EndLogRegion(enabled: true);
			AppMetaData appMetaData2 = appMetaData;
			LogTrace("result = " + appMetaData2.ToString());
			LogInfo("DeserializeMetaDataFrom1_x_x finished: success.");
			return appMetaData;
		}
		catch (MetaDataDeserializeFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("DeserializeMetaDataFrom1_x_x finished: MetaDataDeserializeFailure.");
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
			LogInfo("DeserializeMetaDataFrom1_x_x has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFrom1_x_x failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DeserializeMetaDataFrom1_x_x finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DeserializeMetaDataFrom1_x_xDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DeserializeMetaDataFrom1_x_x(context.Resolve<DeserializeMetaDataFormatVersionDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
