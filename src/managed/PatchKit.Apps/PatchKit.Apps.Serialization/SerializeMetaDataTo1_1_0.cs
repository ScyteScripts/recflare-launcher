using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class SerializeMetaDataTo1_1_0
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

	private struct SerializationData
	{
		[JsonProperty("file_id")]
		public string FileId;

		[JsonProperty("version")]
		public string Version;

		[JsonProperty("hashed_secret")]
		public string HashedSecret;

		[JsonProperty("product_key")]
		public string ProductKey;

		[JsonProperty("product_key_encryption")]
		public string ProductKeyEncryption;

		[JsonProperty("encrypted_license_secret")]
		public string EncryptedLicenseSecret;

		[JsonProperty("encryptedPassword")]
		public string EncryptedPassword;

		[JsonProperty("is_installed")]
		public bool IsInstalled;

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

	private const string FormatVersion = "1.1.0";

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Text Execute(AppMetaData value)
	{
		AppLastFilesOperationTaskStateSerializationData? lastFilesOperationTaskState = null;
		if (value.Info.LastFilesOperationTaskState.HasValue)
		{
			AppLastInstallTaskState? appLastInstallTaskState = value.Info.LastFilesOperationTaskState.Value.AppLastInstallTaskState;
			AppLastUpdateTaskState? appLastUpdateTaskState = value.Info.LastFilesOperationTaskState.Value.AppLastUpdateTaskState;
			AppLastRepairTaskState? appLastRepairTaskState = value.Info.LastFilesOperationTaskState.Value.AppLastRepairTaskState;
			if (appLastInstallTaskState.HasValue)
			{
				lastFilesOperationTaskState = new AppLastFilesOperationTaskStateSerializationData
				{
					AppLastInstallTaskState = new AppLastInstallTaskStateSerializationData
					{
						Progress = (appLastInstallTaskState.Value.Progress.HasValue ? new AppFilesOperationTaskProgressSerializationData?(new AppFilesOperationTaskProgressSerializationData
						{
							BytesCount = appLastInstallTaskState.Value.Progress.Value.PackagesProcessedBytesCount,
							TotalBytesCount = appLastInstallTaskState.Value.Progress.Value.PackagesTotalBytesCount,
							BytesCountPerSecond = appLastInstallTaskState.Value.Progress.Value.PackagesProcessedBytesCountPerSecond
						}) : ((AppFilesOperationTaskProgressSerializationData?)null)),
						Error = appLastInstallTaskState.Value.Error
					}
				};
			}
			else if (appLastUpdateTaskState.HasValue)
			{
				lastFilesOperationTaskState = new AppLastFilesOperationTaskStateSerializationData
				{
					AppLastUpdateTaskState = new AppLastUpdateTaskStateSerializationData
					{
						Progress = (appLastUpdateTaskState.Value.Progress.HasValue ? new AppFilesOperationTaskProgressSerializationData?(new AppFilesOperationTaskProgressSerializationData
						{
							BytesCount = appLastUpdateTaskState.Value.Progress.Value.PackagesProcessedBytesCount,
							TotalBytesCount = appLastUpdateTaskState.Value.Progress.Value.PackagesTotalBytesCount,
							BytesCountPerSecond = appLastUpdateTaskState.Value.Progress.Value.PackagesProcessedBytesCountPerSecond
						}) : ((AppFilesOperationTaskProgressSerializationData?)null)),
						Error = appLastUpdateTaskState.Value.Error
					}
				};
			}
			else
			{
				if (!appLastRepairTaskState.HasValue)
				{
					throw new UnpredictedCondition("Unknown app last files operation task state type.");
				}
				lastFilesOperationTaskState = new AppLastFilesOperationTaskStateSerializationData
				{
					AppLastRepairTaskState = new AppLastRepairTaskStateSerializationData
					{
						Progress = (appLastRepairTaskState.Value.Progress.HasValue ? new AppFilesOperationTaskProgressSerializationData?(new AppFilesOperationTaskProgressSerializationData
						{
							BytesCount = appLastRepairTaskState.Value.Progress.Value.PackagesProcessedBytesCount,
							TotalBytesCount = appLastRepairTaskState.Value.Progress.Value.PackagesTotalBytesCount,
							BytesCountPerSecond = appLastRepairTaskState.Value.Progress.Value.PackagesProcessedBytesCountPerSecond
						}) : ((AppFilesOperationTaskProgressSerializationData?)null)),
						Error = appLastRepairTaskState.Value.Error
					}
				};
			}
		}
		AppLastUnfinishedWriteFileDataOperationInfoSerializationData? lastUnfinishedWriteFileDataOperationInfo = null;
		if (value.Info.LastUnfinishedWriteFileDataOperationInfo.HasValue)
		{
			lastUnfinishedWriteFileDataOperationInfo = new AppLastUnfinishedWriteFileDataOperationInfoSerializationData
			{
				FilePath = value.Info.LastUnfinishedWriteFileDataOperationInfo.Value.FilePath,
				FileVersionId = value.Info.LastUnfinishedWriteFileDataOperationInfo.Value.FileVersionId,
				FileLastBytesCount = value.Info.LastUnfinishedWriteFileDataOperationInfo.Value.FileLastBytesCount
			};
		}
		SerializationData serializationData = new SerializationData
		{
			FileId = "patcher_data",
			Version = "1.1.0",
			HashedSecret = value.Info.HashedSecret?.Value,
			ProductKey = value.Info.LicenseKey?.Value,
			EncryptedPassword = value.Info.EncryptedPassword?.Value,
			EncryptedLicenseSecret = value.Info.EncryptedLicenseSecret?.Value,
			ProductKeyEncryption = "none",
			FileVersionIds = ((IEnumerable<AppMetaDataEntry>)value.Entries).ToDictionary((Func<AppMetaDataEntry, string>)((AppMetaDataEntry e) => e.Path), (Func<AppMetaDataEntry, int>)((AppMetaDataEntry e) => e.VersionId.Value)),
			IsInstalled = value.Info.InstalledVersionId.HasValue,
			InstalledVersionId = value.Info.InstalledVersionId?.Value,
			LastInstalledVersionId = value.Info.LastInstalledVersionId?.Value
		};
		AppPath? appPath = value.Info.StartInfo?.ExecutablePath;
		serializationData.StartExePath = (appPath.HasValue ? ((string)appPath.GetValueOrDefault()) : null);
		Text? text = value.Info.StartInfo?.ExecutableArgs;
		serializationData.StartExeArgs = (text.HasValue ? ((string)text.GetValueOrDefault()) : null);
		serializationData.LastFilesOperationTaskState = lastFilesOperationTaskState;
		serializationData.LastUnfinishedWriteFileDataOperationInfo = lastUnfinishedWriteFileDataOperationInfo;
		SerializationData serializationData2 = serializationData;
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			Converters = { (JsonConverter)new StringEnumConverter(new SnakeCaseNamingStrategy()) }
		};
		string v = JsonConvert.SerializeObject(serializationData2, Formatting.Indented, settings);
		Assert.IsNotNull(v);
		return new Text(v);
	}

	public SerializeMetaDataTo1_1_0(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
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
		LogInfo("Executing SerializeMetaDataTo1_1_0...");
		AppMetaData appMetaData = value;
		LogTrace("value = " + appMetaData.ToString());
		try
		{
			BeginLogRegion(enabled: true);
			Text text = Execute(value);
			EndLogRegion(enabled: true);
			LogTrace("result = " + text);
			LogInfo("SerializeMetaDataTo1_1_0 finished: success.");
			return text;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("SerializeMetaDataTo1_1_0 has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("SerializeMetaDataTo1_1_0 failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("SerializeMetaDataTo1_1_0 finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public SerializeMetaDataTo1_1_0Delegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new SerializeMetaDataTo1_1_0(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
