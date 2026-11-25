using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using PatchKit.Api;
using PatchKit.Api.Properties;
using PatchKit.Apps;
using PatchKit.Apps.Properties;
using PatchKit.Apps.Remote;
using PatchKit.Apps.Remote.Properties;
using PatchKit.Apps.Updating;
using PatchKit.Apps.Updating.Properties;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.Properties;
using PatchKit.Librsync.Properties;
using PatchKit.Network.Properties;
using Sentry;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

public static class LibpkappsNet
{
	public struct GetPatchKitAppInstalledVersionIdResult
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool hasValueField;

		public int value;
	}

	public struct GetPatchKitAppLastFilesOperationTaskStateResult
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool hasValueField;

		public PatchKitAppLastFilesOperationTaskState value;
	}

	public struct GetPatchKitAppLastInstalledVersionIdResult
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool hasValueField;

		public int value;
	}

	public enum OperatingSystemType
	{
		Linux64Bit,
		MacOs64Bit,
		Windows64Bit
	}

	public struct PatchKitAppFilesOperationTaskProgress
	{
		public long packagesProcessedBytesCount;

		public long packagesTotalBytesCount;

		public long packagesProcessedBytesCountPerSecond;

		public long filesProcessedBytesCount;

		public long filesTotalBytesCount;

		public long filesProcessedBytesCountPerSecond;
	}

	public enum PatchKitAppInstallTaskError
	{
		Fatal,
		Cancelled,
		OutOfPatchKitAppDirFreeSpace,
		UnauthorizedAccessToPatchKitAppDir,
		RequiredPatchKitAppLicenseKey,
		InvalidPatchKitAppLicenseKey,
		BlockedPatchKitAppLicenseKey,
		InvalidPatchKitAppAuthKey,
		ExpiredPatchKitAppAuthKey,
		InvalidPatchKitAppPassword
	}

	public struct PatchKitAppInstallTaskState
	{
		public int id;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasProgressField;

		public PatchKitAppFilesOperationTaskProgress progress;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasFinished;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppInstallTaskError error;
	}

	public struct PatchKitAppLastFilesOperationTaskState
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool hasPatchKitAppLastInstallTaskStateField;

		public PatchKitAppLastInstallTaskState patchKitAppLastInstallTaskState;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasPatchKitAppLastUpdateTaskStateField;

		public PatchKitAppLastUpdateTaskState patchKitAppLastUpdateTaskState;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasPatchKitAppLastRepairTaskStateField;

		public PatchKitAppLastRepairTaskState patchKitAppLastRepairTaskState;
	}

	public struct PatchKitAppLastInstallTaskState
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool hasProgressField;

		public PatchKitAppFilesOperationTaskProgress progress;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppInstallTaskError error;
	}

	public struct PatchKitAppLastRepairTaskState
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool hasProgressField;

		public PatchKitAppFilesOperationTaskProgress progress;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppRepairTaskError error;
	}

	public struct PatchKitAppLastUpdateTaskState
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool hasProgressField;

		public PatchKitAppFilesOperationTaskProgress progress;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppUpdateTaskError error;
	}

	public enum PatchKitAppMoveTaskError
	{
		Fatal,
		Cancelled,
		OutOfPatchKitAppNewDirFreeSpace,
		UnauthorizedAccessToPatchKitAppDir,
		UnauthorizedAccessToPatchKitAppNewDir
	}

	public struct PatchKitAppMoveTaskState
	{
		public int id;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasFinished;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppMoveTaskError error;
	}

	public enum PatchKitAppRepairTaskError
	{
		Fatal,
		Cancelled,
		OutOfPatchKitAppDirFreeSpace,
		UnauthorizedAccessToPatchKitAppDir,
		RequiredPatchKitAppLicenseKey,
		InvalidPatchKitAppLicenseKey,
		BlockedPatchKitAppLicenseKey,
		InvalidPatchKitAppAuthKey,
		ExpiredPatchKitAppAuthKey,
		InvalidPatchKitAppPassword
	}

	public struct PatchKitAppRepairTaskState
	{
		public int id;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasProgressField;

		public PatchKitAppFilesOperationTaskProgress progress;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasFinished;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppRepairTaskError error;
	}

	public enum PatchKitAppUninstallTaskError
	{
		Fatal,
		Cancelled,
		UnauthorizedAccessToPatchKitAppDir
	}

	public struct PatchKitAppUninstallTaskState
	{
		public int id;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasFinished;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppUninstallTaskError error;
	}

	public enum PatchKitAppUpdateTaskError
	{
		Fatal,
		Cancelled,
		OutOfPatchKitAppDirFreeSpace,
		UnauthorizedAccessToPatchKitAppDir,
		RequiredPatchKitAppLicenseKey,
		InvalidPatchKitAppLicenseKey,
		BlockedPatchKitAppLicenseKey,
		InvalidPatchKitAppAuthKey,
		ExpiredPatchKitAppAuthKey,
		InvalidPatchKitAppPassword
	}

	public struct PatchKitAppUpdateTaskState
	{
		public int id;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasProgressField;

		public PatchKitAppFilesOperationTaskProgress progress;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasFinished;

		[MarshalAs(UnmanagedType.U1)]
		public bool hasErrorField;

		public PatchKitAppUpdateTaskError error;
	}

	public class Program
	{
		public static void Main()
		{
			Console.WriteLine("This assembly is not meant to be run directly.");
			Console.WriteLine("Instead, please use the SampleHost process to load this assembly.");
		}
	}

	public struct StartPatchKitAppResult
	{
		public int patchKitAppProcessId;
	}

	public struct StartPatchKitAppInstallTaskResult
	{
		public int patchKitAppInstallTaskId;
	}

	public struct StartPatchKitAppMoveTaskResult
	{
		public int patchKitAppMoveTaskId;
	}

	public struct StartPatchKitAppRepairTaskResult
	{
		public int patchKitAppRepairTaskId;
	}

	public struct StartPatchKitAppUninstallTaskResult
	{
		public int patchKitAppUninstallTaskId;
	}

	public struct StartPatchKitAppUpdateTaskResult
	{
		public int patchKitAppUpdateTaskId;
	}

	private class _LibpkappsNetContext : IDisposable
	{
		private readonly string _libpkappsNetLogFilePath;

		private FileStream _libpkappsNetLogFileStream;

		private System.IO.Compression.GZipStream _libpkappsNetLogFileGzipStream;

		private StreamWriter _libpkappsNetLogFileWriter;

		private readonly IContainer _container;

		private FunctionFatalError _containerFuncFatalError;

		private void WriteToConsole(string text)
		{
			try
			{
				Console.WriteLine(text);
			}
			catch
			{
			}
		}

		private void WriteToLibpkappsNetLogFile(string text)
		{
			try
			{
				if (_libpkappsNetLogFileWriter == null)
				{
					WriteToConsole(text);
					return;
				}
				lock (_libpkappsNetLogFileWriter)
				{
					_libpkappsNetLogFileWriter.WriteLine(text);
					_libpkappsNetLogFileWriter.Flush();
					_libpkappsNetLogFileGzipStream.Flush();
					_libpkappsNetLogFileStream.Flush();
				}
			}
			catch
			{
				WriteToConsole(text);
			}
		}

		public _LibpkappsNetContext(string libpkappsNetContextName)
		{
			WriteToConsole("Creating libpkapps.NET " + libpkappsNetContextName + " context...");
			if (!_isLibpkappsNetInitialized)
			{
				throw new InvalidOperationException("libpkapps.NET has not been initialized.");
			}
			PlatformType platformType = _operatingSystemType switch
			{
				OperatingSystemType.Linux64Bit => PlatformType.Linux64, 
				OperatingSystemType.MacOs64Bit => PlatformType.Osx64, 
				OperatingSystemType.Windows64Bit => PlatformType.Win64, 
				_ => throw new InvalidOperationException($"Invalid operating system type - {(int)_operatingSystemType}"), 
			};
			string path = "libpkapps_" + libpkappsNetContextName;
			try
			{
				WriteToConsole("Clearing previous libpkapps.NET log files...");
				FileInfo[] files = new DirectoryInfo(_libpkappsNetLogsDirPath).GetFiles("libpkapps_*.log.gz");
				DateTime now = DateTime.Now;
				TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0);
				FileInfo[] array = files;
				foreach (FileInfo fileInfo in array)
				{
					TimeSpan timeSpan2 = now - fileInfo.LastWriteTime;
					if (timeSpan2 > timeSpan)
					{
						File.Delete(fileInfo.FullName);
					}
				}
			}
			catch (Exception ex)
			{
				WriteToConsole("Failed to clear libpkapps.NET log files.");
				WriteToConsole(ex.ToString());
			}
			string arg = System.IO.Path.Combine(_libpkappsNetLogsDirPath, path);
			try
			{
				WriteToConsole("Creating libpkapps.NET log file...");
				Directory.CreateDirectory(_libpkappsNetLogsDirPath);
				uint num = 0u;
				do
				{
					_libpkappsNetLogFilePath = $"{arg}.{num++}.log.gz";
				}
				while (File.Exists(_libpkappsNetLogFilePath));
				_libpkappsNetLogFileStream = new FileStream(_libpkappsNetLogFilePath, FileMode.Create);
				_libpkappsNetLogFileGzipStream = new System.IO.Compression.GZipStream(_libpkappsNetLogFileStream, System.IO.Compression.CompressionMode.Compress);
				_libpkappsNetLogFileWriter = new StreamWriter(_libpkappsNetLogFileGzipStream);
				WriteToConsole("libpkapps.NET log file created at " + _libpkappsNetLogFilePath);
			}
			catch (Exception ex2)
			{
				WriteToConsole("Failed to create libpkapps.NET log file.");
				WriteToConsole(ex2.ToString());
			}
			int libpkappsNetLogIndentLevel = 0;
			string libpkappsNetLogIndent = "";
			int libpkappsNetLogDisabledLevel = 0;
			bool isLibpkappsNetLogDisbled = false;
			PatchKit.Core.Properties.AssemblyModule assemblyModule = new PatchKit.Core.Properties.AssemblyModule(platformType, (LogMessage x) =>
			{
				if (!isLibpkappsNetLogDisbled || x.Type == LogMessageType.Error)
				{
					WriteToLibpkappsNetLogFile("[" + DateTime.Now.ToString("o") + "]" + libpkappsNetLogIndent + x.Message.Replace("\n", "\n" + libpkappsNetLogIndent));
					if (x.Exception != null)
					{
						WriteToLibpkappsNetLogFile("[" + DateTime.Now.ToString("o") + "]" + libpkappsNetLogIndent + x.Exception.ToString().Replace("\n", "\n" + libpkappsNetLogIndent));
					}
				}
			}, (bool enabled) =>
			{
				libpkappsNetLogIndentLevel++;
				if (!enabled)
				{
					libpkappsNetLogDisabledLevel++;
				}
				RefreshLogIndent();
				isLibpkappsNetLogDisbled = libpkappsNetLogDisabledLevel > 0;
			}, (bool enabled) =>
			{
				libpkappsNetLogIndentLevel--;
				if (!enabled)
				{
					libpkappsNetLogDisabledLevel--;
				}
				RefreshLogIndent();
				isLibpkappsNetLogDisbled = libpkappsNetLogDisabledLevel > 0;
			});
			PatchKit.Network.Properties.AssemblyModule assemblyModule2 = new PatchKit.Network.Properties.AssemblyModule(assemblyModule);
			ApiConnectionSettings apiConnectionSettings = PatchKit.Api.Properties.AssemblyModule.DefaultApiConnectionSettings;
			if (_patchKitApiUrl != null)
			{
				Uri uri = new Uri(_patchKitApiUrl);
				Uri uri2 = ((_patchKitApiCacheUrl != null) ? new Uri(_patchKitApiCacheUrl) : null);
				ApiConnectionServer mainServer = new ApiConnectionServer(uri.Host, uri.LocalPath, uri.Port, uri.Scheme == Uri.UriSchemeHttps);
				ApiConnectionServer? apiConnectionServer = ((uri2 != null) ? new ApiConnectionServer?(new ApiConnectionServer(uri2.Host, uri2.LocalPath, uri2.Port, uri2.Scheme == Uri.UriSchemeHttps)) : ((ApiConnectionServer?)null));
				apiConnectionSettings = new ApiConnectionSettings(mainServer, new ImmutableArray<ApiConnectionServer>((!apiConnectionServer.HasValue) ? new ApiConnectionServer[0] : new ApiConnectionServer[1] { apiConnectionServer.Value }));
			}
			ApiConnectionSettings keysApiConnectionSettings = PatchKit.Api.Properties.AssemblyModule.DefaultKeysApiConnectionSettings;
			if (_patchKitKeysApiUrl != null)
			{
				Uri uri3 = new Uri(_patchKitKeysApiUrl);
				ApiConnectionServer mainServer2 = new ApiConnectionServer(uri3.Host, uri3.LocalPath, uri3.Port, uri3.Scheme == Uri.UriSchemeHttps);
				keysApiConnectionSettings = new ApiConnectionSettings(mainServer2, ImmutableArray<ApiConnectionServer>.Empty);
			}
			PatchKit.Api.Properties.AssemblyModule module = new PatchKit.Api.Properties.AssemblyModule("libpkapps:" + _LibpkappsNetVersion.Text + ";" + _patchKitApiClientId, apiConnectionSettings, keysApiConnectionSettings, assemblyModule, assemblyModule2);
			PatchKit.Apps.Properties.AssemblyModule assemblyModule3 = new PatchKit.Apps.Properties.AssemblyModule(assemblyModule);
			PatchKit.Apps.Remote.Properties.AssemblyModule assemblyModule4 = new PatchKit.Apps.Remote.Properties.AssemblyModule(assemblyModule);
			PatchKit.Apps.Updating.Properties.AssemblyModule module2 = new PatchKit.Apps.Updating.Properties.AssemblyModule(assemblyModule, assemblyModule2, assemblyModule3, assemblyModule4);
			PatchKit.Librsync.Properties.AssemblyModule module3 = new PatchKit.Librsync.Properties.AssemblyModule();
			ContainerBuilder containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterModule(assemblyModule);
			containerBuilder.RegisterModule(assemblyModule2);
			containerBuilder.RegisterModule(module);
			containerBuilder.RegisterModule(module3);
			containerBuilder.RegisterModule(assemblyModule3);
			containerBuilder.RegisterModule(assemblyModule4);
			containerBuilder.RegisterModule(module2);
			_container = containerBuilder.Build();
			WriteToConsole("libpkapps.NET context " + libpkappsNetContextName + " successfuly created.");
			void RefreshLogIndent()
			{
				libpkappsNetLogIndent = "";
				for (int j = 0; j < libpkappsNetLogIndentLevel; j++)
				{
					libpkappsNetLogIndent += "    ";
				}
			}
		}

		public T ContainerResolve<T>()
		{
			return _container.Resolve<T>();
		}

		public void SetContainerFuncFatalError(FunctionFatalError funcFatalError)
		{
			_containerFuncFatalError = funcFatalError;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~_LibpkappsNetContext()
		{
			Dispose(disposing: false);
		}

		private void Dispose(bool disposing)
		{
			try
			{
				WriteToConsole("Disposing libpkapps.NET context...");
				if (_libpkappsNetLogFileWriter != null)
				{
					if (_containerFuncFatalError != null && Environment.GetEnvironmentVariable("STUDIO_UPDATER_LEGACY_TELEMETRY") == "1")
					{
						WriteToLibpkappsNetLogFile(_containerFuncFatalError.ToString());
					}
					_libpkappsNetLogFileWriter.Flush();
					_libpkappsNetLogFileWriter.Dispose();
					_libpkappsNetLogFileWriter = null;
					_libpkappsNetLogFileGzipStream.Dispose();
					_libpkappsNetLogFileGzipStream = null;
					_libpkappsNetLogFileStream.Dispose();
					_libpkappsNetLogFileStream = null;
				}
				if (disposing)
				{
					WriteToConsole("Managed disposing libpkapps.NET context...");
					if (_containerFuncFatalError != null && Environment.GetEnvironmentVariable("STUDIO_UPDATER_LEGACY_TELEMETRY") == "1")
					{
						string text = null;
						try
						{
							WriteToConsole("Uploading libpkapps.NET log...");
							if (_libpkappsNetLogFilePath == null || !File.Exists(_libpkappsNetLogFilePath))
							{
								WriteToConsole("libpkapps.NET log doesn't exist, it won't be uploaded.");
								return;
							}
							text = _uploadLibpkappsNetLogAsync(_libpkappsNetLogFilePath, Guid.NewGuid().ToString()).Result;
							WriteToConsole("libpkapps.NET log has been uploaded and is available at " + text);
						}
						catch (Exception ex)
						{
							WriteToConsole("Failed to upload libpkapps.NET log.");
							WriteToConsole(ex.ToString());
						}
						try
						{
							WriteToConsole("Sending libpkapps.NET Sentry event...");
							using (SentryClient sentryClient = _getLibpkappsNetSentryClient())
							{
								SentryEvent sentryEvent = new SentryEvent(_containerFuncFatalError.InnerException);
								if (text != null)
								{
									sentryEvent.SetExtra("log_url", text);
								}
								sentryClient.CaptureEvent(sentryEvent);
							}
							WriteToConsole("libpkapps.NET Sentry event sent.");
						}
						catch (Exception ex2)
						{
							WriteToConsole("Failed to send libpkapps.NET Sentry event.");
							WriteToConsole(ex2.ToString());
						}
					}
					WriteToConsole("libpkapps.NET context disposed.");
				}
				WriteToConsole("libpkapps.NET context disposed.");
			}
			catch (Exception ex3)
			{
				WriteToConsole("Disposing libpkapps.NET context failed.");
				WriteToConsole(ex3.ToString());
			}
		}
	}

	private static class _LibpkappsNetVersion
	{
		public const int Major = 0;

		public const int Minor = 41;

		public const int Patch = 6;

		public const string Suffix = "";

		public static readonly string Text = string.Format("{0}.{1}.{2}{3}", 0, 41, 6, "");
	}

	private struct RequestPutUrlContent
	{
		[JsonProperty("app_id")]
		public string AppId { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("priority")]
		public string Priority { get; set; }

		[JsonProperty("guid")]
		public string Guid { get; set; }

		[JsonProperty("compression")]
		public string Compression { get; set; }
	}

	private struct RequestPutUrlResponseData
	{
		[JsonProperty("url")]
		public string Url { get; set; }
	}

	private static bool _isLibpkappsNetInitialized = false;

	private static string _libpkappsNetLogsDirPath;

	private static OperatingSystemType _operatingSystemType;

	private static string _patchKitApiClientId;

	private static string _patchKitApiUrl;

	private static string _patchKitApiCacheUrl;

	private static string _patchKitKeysApiUrl;

	private static AppTasksState _containerAppTasksState = new AppTasksState();

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string cancelPatchKitAppInstallTask(int patchKitAppInstallTaskId)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("cancelPatchKitAppInstallTask");
		try
		{
			AppInstallTaskState appInstallTaskState = _containerAppTasksState.AppInstallTasks.FirstOrDefault((AppInstallTaskState x) => x.Id == patchKitAppInstallTaskId);
			appInstallTaskState.CancellationTokenSource.Cancel();
			return null;
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string cancelPatchKitAppRepairTask(int patchKitAppRepairTaskId)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("cancelPatchKitAppRepairTask");
		try
		{
			AppRepairTaskState appRepairTaskState = _containerAppTasksState.AppRepairTasks.FirstOrDefault((AppRepairTaskState x) => x.Id == patchKitAppRepairTaskId);
			appRepairTaskState.CancellationTokenSource.Cancel();
			return null;
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string cancelPatchKitAppUpdateTask(int patchKitAppUpdateTaskId)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("cancelPatchKitAppUpdateTask");
		try
		{
			AppUpdateTaskState appUpdateTaskState = _containerAppTasksState.AppUpdateTasks.FirstOrDefault((AppUpdateTaskState x) => x.Id == patchKitAppUpdateTaskId);
			appUpdateTaskState.CancellationTokenSource.Cancel();
			return null;
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string createPatchKitAppDir([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.U1)] bool isOverwritingOtherPatchKitAppDirAllowed, [MarshalAs(UnmanagedType.U1)] bool isPatchKitAppNotEmptyDirAllowed)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("createAppDir");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App app = new App(path);
			CreateAppRootDirDelegate createAppRootDirDelegate = libpkappsNetContext.ContainerResolve<CreateAppRootDirDelegate>();
			createAppRootDirDelegate(app, isOverwritingOtherPatchKitAppDirAllowed, isPatchKitAppNotEmptyDirAllowed, null, null, null, null);
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FileIsConflictingWithAppRootDir)
		{
			return "file_is_conflicting_with_patchkit_app_dir";
		}
		catch (OtherAppRootDirIsConflictingWithAppRootDir)
		{
			return "other_patchkit_app_dir_is_conflicting_with_patchkit_app_dir";
		}
		catch (AppRootDirIsNotEmpty)
		{
			return "patchkit_app_dir_is_not_empty";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getDoesPatchKitAppDirExist([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.U1)] ref bool result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getDoesPatchKitAppDirExists");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App app = new App(path);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			bool flag = doesAppRootDirExistDelegate(app, null);
			result = flag;
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getDoesPatchKitAppDirHasFreeSpaceForInstallTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppSecret, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppLicenseKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppAuthKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppPassword, ref bool result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getDoesPatchKitAppDirExists");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			AppSecret secret = new AppSecret(patchKitAppSecret);
			AppLicenseKey? appLicenseKey = ((patchKitAppLicenseKey == null) ? ((AppLicenseKey?)null) : new AppLicenseKey?(new AppLicenseKey(patchKitAppLicenseKey)));
			AppAuthKey? authKey = ((patchKitAppAuthKey == null) ? ((AppAuthKey?)null) : new AppAuthKey?(new AppAuthKey(patchKitAppAuthKey)));
			AppPassword? appPassword = ((patchKitAppPassword == null) ? ((AppPassword?)null) : new AppPassword?(new AppPassword(patchKitAppPassword)));
			App app = new App(path);
			AppRemote appRemote = new AppRemote(secret, authKey);
			DoesDirHaveFreeSpaceForUpdatingAppDelegate doesDirHaveFreeSpaceForUpdatingAppDelegate = libpkappsNetContext.ContainerResolve<DoesDirHaveFreeSpaceForUpdatingAppDelegate>();
			bool flag = doesDirHaveFreeSpaceForUpdatingAppDelegate(app, appRemote, appLicenseKey, appPassword, AppValidationMode.MetaData, default, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null, null, null, null, null, null, null, null, null);
			result = flag;
			return null;
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (InvalidAppAuthKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_auth_key_is_invalid";
		}
		catch (ExpiredAppAuthKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_auth_key_is_expired";
		}
		catch (AppLicenseKeyRequired)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_required";
		}
		catch (InvalidAppLicenseKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_invalid";
		}
		catch (BlockedAppLicenseKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_blocked";
		}
		catch (InvalidAppPassword)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_password_is_invalid";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getDoesPatchKitAppDirHasFreeSpaceForRepairTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppSecret, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppLicenseKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppAuthKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppPassword, ref bool result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getDoesPatchKitAppDirExists");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			AppSecret secret = new AppSecret(patchKitAppSecret);
			AppLicenseKey? appLicenseKey = ((patchKitAppLicenseKey == null) ? ((AppLicenseKey?)null) : new AppLicenseKey?(new AppLicenseKey(patchKitAppLicenseKey)));
			AppAuthKey? authKey = ((patchKitAppAuthKey == null) ? ((AppAuthKey?)null) : new AppAuthKey?(new AppAuthKey(patchKitAppAuthKey)));
			AppPassword? appPassword = ((patchKitAppPassword == null) ? ((AppPassword?)null) : new AppPassword?(new AppPassword(patchKitAppPassword)));
			App app = new App(path);
			AppRemote appRemote = new AppRemote(secret, authKey);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			if (!doesAppRootDirExistDelegate(app, null))
			{
				throw new AppRootDirDoesNotExist();
			}
			DoesDirHaveFreeSpaceForUpdatingAppDelegate doesDirHaveFreeSpaceForUpdatingAppDelegate = libpkappsNetContext.ContainerResolve<DoesDirHaveFreeSpaceForUpdatingAppDelegate>();
			bool flag = doesDirHaveFreeSpaceForUpdatingAppDelegate(app, appRemote, appLicenseKey, appPassword, AppValidationMode.Hashes, default, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null, null, null, null, null, null, null, null, null);
			result = flag;
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (InvalidAppAuthKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_auth_key_is_invalid";
		}
		catch (ExpiredAppAuthKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_auth_key_is_expired";
		}
		catch (AppLicenseKeyRequired)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_required";
		}
		catch (InvalidAppLicenseKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_invalid";
		}
		catch (BlockedAppLicenseKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_blocked";
		}
		catch (InvalidAppPassword)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_password_is_invalid";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getDoesPatchKitAppDirHasFreeSpaceForUpdateTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppSecret, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppLicenseKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppAuthKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppPassword, ref bool result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getDoesPatchKitAppDirExists");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			AppSecret secret = new AppSecret(patchKitAppSecret);
			AppLicenseKey? appLicenseKey = ((patchKitAppLicenseKey == null) ? ((AppLicenseKey?)null) : new AppLicenseKey?(new AppLicenseKey(patchKitAppLicenseKey)));
			AppAuthKey? authKey = ((patchKitAppAuthKey == null) ? ((AppAuthKey?)null) : new AppAuthKey?(new AppAuthKey(patchKitAppAuthKey)));
			AppPassword? appPassword = ((patchKitAppPassword == null) ? ((AppPassword?)null) : new AppPassword?(new AppPassword(patchKitAppPassword)));
			App app = new App(path);
			AppRemote appRemote = new AppRemote(secret, authKey);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			if (!doesAppRootDirExistDelegate(app, null))
			{
				throw new AppRootDirDoesNotExist();
			}
			DoesAppNeedRepairDelegate doesAppNeedRepairDelegate = libpkappsNetContext.ContainerResolve<DoesAppNeedRepairDelegate>();
			if (doesAppNeedRepairDelegate(app, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null))
			{
				throw new AppNeedsRepair();
			}
			DoesDirHaveFreeSpaceForUpdatingAppDelegate doesDirHaveFreeSpaceForUpdatingAppDelegate = libpkappsNetContext.ContainerResolve<DoesDirHaveFreeSpaceForUpdatingAppDelegate>();
			bool flag = doesDirHaveFreeSpaceForUpdatingAppDelegate(app, appRemote, appLicenseKey, appPassword, AppValidationMode.MetaData, default, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null, null, null, null, null, null, null, null, null);
			result = flag;
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (AppNeedsRepair)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_needs_repair";
		}
		catch (InvalidAppAuthKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_auth_key_is_invalid";
		}
		catch (ExpiredAppAuthKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_auth_key_is_expired";
		}
		catch (AppLicenseKeyRequired)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_required";
		}
		catch (InvalidAppLicenseKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_invalid";
		}
		catch (BlockedAppLicenseKey)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_license_key_is_blocked";
		}
		catch (InvalidAppPassword)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_password_is_invalid";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getDoesPatchKitAppNeedRepair([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.U1)] ref bool result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getDoesPatchKitAppNeedRepair");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App app = new App(path);
			DoesAppNeedRepairDelegate doesAppNeedRepairDelegate = libpkappsNetContext.ContainerResolve<DoesAppNeedRepairDelegate>();
			bool flag = doesAppNeedRepairDelegate(app, null, null);
			result = flag;
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppInstalledVersionId([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, ref GetPatchKitAppInstalledVersionIdResult result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getPatchKitAppInstalledVersionId");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App app = new App(path);
			GetAppInstalledVersionIdDelegate getAppInstalledVersionIdDelegate = libpkappsNetContext.ContainerResolve<GetAppInstalledVersionIdDelegate>();
			AppVersionId? appVersionId = getAppInstalledVersionIdDelegate(app, null, null);
			result = new GetPatchKitAppInstalledVersionIdResult
			{
				hasValueField = appVersionId.HasValue,
				value = (appVersionId.HasValue ? ((int)appVersionId.Value) : 0)
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppInstallTaskState(int patchKitAppInstallTaskId, ref PatchKitAppInstallTaskState result)
	{
		try
		{
			AppInstallTaskState appInstallTaskState = _containerAppTasksState.AppInstallTasks.FirstOrDefault((AppInstallTaskState x) => x.Id == patchKitAppInstallTaskId);
			result = new PatchKitAppInstallTaskState
			{
				id = appInstallTaskState.Id,
				hasProgressField = appInstallTaskState.Progress.HasValue,
				progress = (appInstallTaskState.Progress.HasValue ? new PatchKitAppFilesOperationTaskProgress
				{
					packagesProcessedBytesCount = appInstallTaskState.Progress.Value.PackagesProcessedBytesCount,
					packagesTotalBytesCount = appInstallTaskState.Progress.Value.PackagesTotalBytesCount,
					packagesProcessedBytesCountPerSecond = appInstallTaskState.Progress.Value.PackagesProcessedBytesCountPerSecond,
					filesProcessedBytesCount = appInstallTaskState.Progress.Value.FilesProcessedBytesCount,
					filesTotalBytesCount = appInstallTaskState.Progress.Value.FilesTotalBytesCount,
					filesProcessedBytesCountPerSecond = appInstallTaskState.Progress.Value.FilesProcessedBytesCountPerSecond
				} : default(PatchKitAppFilesOperationTaskProgress)),
				hasErrorField = appInstallTaskState.Error.HasValue,
				error = (PatchKitAppInstallTaskError)(appInstallTaskState.Error.HasValue ? appInstallTaskState.Error.Value : AppInstallTaskError.Fatal),
				hasFinished = appInstallTaskState.HasFinished
			};
			return null;
		}
		catch (Exception)
		{
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppLastFilesOperationTaskState([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, ref GetPatchKitAppLastFilesOperationTaskStateResult result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getPatchKitAppLastFilesOperationTaskState");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App app = new App(path);
			GetAppLastFilesOperationTaskStateDelegate getAppLastFilesOperationTaskStateDelegate = libpkappsNetContext.ContainerResolve<GetAppLastFilesOperationTaskStateDelegate>();
			AppLastFilesOperationTaskState? appLastFilesOperationTaskState = getAppLastFilesOperationTaskStateDelegate(app, null, null);
			AppLastInstallTaskState? appLastInstallTaskState = appLastFilesOperationTaskState?.AppLastInstallTaskState;
			AppLastUpdateTaskState? appLastUpdateTaskState = appLastFilesOperationTaskState?.AppLastUpdateTaskState;
			AppLastRepairTaskState? appLastRepairTaskState = appLastFilesOperationTaskState?.AppLastRepairTaskState;
			result = new GetPatchKitAppLastFilesOperationTaskStateResult
			{
				hasValueField = appLastFilesOperationTaskState.HasValue,
				value = (appLastFilesOperationTaskState.HasValue ? new PatchKitAppLastFilesOperationTaskState
				{
					hasPatchKitAppLastInstallTaskStateField = appLastInstallTaskState.HasValue,
					patchKitAppLastInstallTaskState = (appLastInstallTaskState.HasValue ? new PatchKitAppLastInstallTaskState
					{
						hasProgressField = appLastInstallTaskState.Value.Progress.HasValue,
						progress = (appLastInstallTaskState.Value.Progress.HasValue ? new PatchKitAppFilesOperationTaskProgress
						{
							packagesProcessedBytesCount = appLastInstallTaskState.Value.Progress.Value.PackagesProcessedBytesCount,
							packagesTotalBytesCount = appLastInstallTaskState.Value.Progress.Value.PackagesTotalBytesCount,
							packagesProcessedBytesCountPerSecond = appLastInstallTaskState.Value.Progress.Value.PackagesProcessedBytesCountPerSecond,
							filesProcessedBytesCount = appLastInstallTaskState.Value.Progress.Value.FilesProcessedBytesCount,
							filesTotalBytesCount = appLastInstallTaskState.Value.Progress.Value.FilesTotalBytesCount,
							filesProcessedBytesCountPerSecond = appLastInstallTaskState.Value.Progress.Value.FilesProcessedBytesCountPerSecond
						} : default(PatchKitAppFilesOperationTaskProgress)),
						hasErrorField = appLastInstallTaskState.Value.Error.HasValue,
						error = (PatchKitAppInstallTaskError)(appLastInstallTaskState.Value.Error.HasValue ? appLastInstallTaskState.Value.Error.Value : AppInstallTaskError.Fatal)
					} : default(PatchKitAppLastInstallTaskState)),
					hasPatchKitAppLastUpdateTaskStateField = appLastUpdateTaskState.HasValue,
					patchKitAppLastUpdateTaskState = (appLastUpdateTaskState.HasValue ? new PatchKitAppLastUpdateTaskState
					{
						hasProgressField = appLastUpdateTaskState.Value.Progress.HasValue,
						progress = (appLastUpdateTaskState.Value.Progress.HasValue ? new PatchKitAppFilesOperationTaskProgress
						{
							packagesProcessedBytesCount = appLastUpdateTaskState.Value.Progress.Value.PackagesProcessedBytesCount,
							packagesTotalBytesCount = appLastUpdateTaskState.Value.Progress.Value.PackagesTotalBytesCount,
							packagesProcessedBytesCountPerSecond = appLastUpdateTaskState.Value.Progress.Value.PackagesProcessedBytesCountPerSecond,
							filesProcessedBytesCount = appLastUpdateTaskState.Value.Progress.Value.FilesProcessedBytesCount,
							filesTotalBytesCount = appLastUpdateTaskState.Value.Progress.Value.FilesTotalBytesCount,
							filesProcessedBytesCountPerSecond = appLastUpdateTaskState.Value.Progress.Value.FilesProcessedBytesCountPerSecond
						} : default(PatchKitAppFilesOperationTaskProgress)),
						hasErrorField = appLastUpdateTaskState.Value.Error.HasValue,
						error = (PatchKitAppUpdateTaskError)(appLastUpdateTaskState.Value.Error.HasValue ? appLastUpdateTaskState.Value.Error.Value : AppUpdateTaskError.Fatal)
					} : default(PatchKitAppLastUpdateTaskState)),
					hasPatchKitAppLastRepairTaskStateField = appLastRepairTaskState.HasValue,
					patchKitAppLastRepairTaskState = (appLastRepairTaskState.HasValue ? new PatchKitAppLastRepairTaskState
					{
						hasProgressField = appLastRepairTaskState.Value.Progress.HasValue,
						progress = (appLastRepairTaskState.Value.Progress.HasValue ? new PatchKitAppFilesOperationTaskProgress
						{
							packagesProcessedBytesCount = appLastRepairTaskState.Value.Progress.Value.PackagesProcessedBytesCount,
							packagesTotalBytesCount = appLastRepairTaskState.Value.Progress.Value.PackagesTotalBytesCount,
							packagesProcessedBytesCountPerSecond = appLastRepairTaskState.Value.Progress.Value.PackagesProcessedBytesCountPerSecond,
							filesProcessedBytesCount = appLastRepairTaskState.Value.Progress.Value.FilesProcessedBytesCount,
							filesTotalBytesCount = appLastRepairTaskState.Value.Progress.Value.FilesTotalBytesCount,
							filesProcessedBytesCountPerSecond = appLastRepairTaskState.Value.Progress.Value.FilesProcessedBytesCountPerSecond
						} : default(PatchKitAppFilesOperationTaskProgress)),
						hasErrorField = appLastRepairTaskState.Value.Error.HasValue,
						error = (PatchKitAppRepairTaskError)(appLastRepairTaskState.Value.Error.HasValue ? appLastRepairTaskState.Value.Error.Value : AppRepairTaskError.Fatal)
					} : default(PatchKitAppLastRepairTaskState))
				} : default(PatchKitAppLastFilesOperationTaskState))
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppLastInstalledVersionId([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, ref GetPatchKitAppLastInstalledVersionIdResult result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("getPatchKitAppLastInstalledVersionId");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App app = new App(path);
			GetAppLastInstalledVersionIdDelegate getAppLastInstalledVersionIdDelegate = libpkappsNetContext.ContainerResolve<GetAppLastInstalledVersionIdDelegate>();
			AppVersionId? appVersionId = getAppLastInstalledVersionIdDelegate(app, null, null);
			result = new GetPatchKitAppLastInstalledVersionIdResult
			{
				hasValueField = appVersionId.HasValue,
				value = (appVersionId.HasValue ? ((int)appVersionId.Value) : 0)
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppMoveTaskState(int patchKitAppMoveTaskId, ref PatchKitAppMoveTaskState result)
	{
		try
		{
			AppMoveTaskState appMoveTaskState = _containerAppTasksState.AppMoveTasks.FirstOrDefault((AppMoveTaskState x) => x.Id == patchKitAppMoveTaskId);
			result = new PatchKitAppMoveTaskState
			{
				id = appMoveTaskState.Id,
				hasErrorField = appMoveTaskState.Error.HasValue,
				error = (PatchKitAppMoveTaskError)(appMoveTaskState.Error.HasValue ? appMoveTaskState.Error.Value : AppMoveTaskError.Fatal),
				hasFinished = appMoveTaskState.HasFinished
			};
			return null;
		}
		catch (Exception)
		{
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppRepairTaskState(int patchKitAppRepairTaskId, ref PatchKitAppRepairTaskState result)
	{
		try
		{
			AppRepairTaskState appRepairTaskState = _containerAppTasksState.AppRepairTasks.FirstOrDefault((AppRepairTaskState x) => x.Id == patchKitAppRepairTaskId);
			result = new PatchKitAppRepairTaskState
			{
				id = appRepairTaskState.Id,
				hasProgressField = appRepairTaskState.Progress.HasValue,
				progress = (appRepairTaskState.Progress.HasValue ? new PatchKitAppFilesOperationTaskProgress
				{
					packagesProcessedBytesCount = appRepairTaskState.Progress.Value.PackagesProcessedBytesCount,
					packagesTotalBytesCount = appRepairTaskState.Progress.Value.PackagesTotalBytesCount,
					packagesProcessedBytesCountPerSecond = appRepairTaskState.Progress.Value.PackagesProcessedBytesCountPerSecond,
					filesProcessedBytesCount = appRepairTaskState.Progress.Value.FilesProcessedBytesCount,
					filesTotalBytesCount = appRepairTaskState.Progress.Value.FilesTotalBytesCount,
					filesProcessedBytesCountPerSecond = appRepairTaskState.Progress.Value.FilesProcessedBytesCountPerSecond
				} : default(PatchKitAppFilesOperationTaskProgress)),
				hasErrorField = appRepairTaskState.Error.HasValue,
				error = (PatchKitAppRepairTaskError)(appRepairTaskState.Error.HasValue ? appRepairTaskState.Error.Value : AppRepairTaskError.Fatal),
				hasFinished = appRepairTaskState.HasFinished
			};
			return null;
		}
		catch (Exception)
		{
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppUninstallTaskState(int patchKitAppUninstallTaskId, ref PatchKitAppUninstallTaskState result)
	{
		try
		{
			AppUninstallTaskState appUninstallTaskState = _containerAppTasksState.AppUninstallTasks.FirstOrDefault((AppUninstallTaskState x) => x.Id == patchKitAppUninstallTaskId);
			result = new PatchKitAppUninstallTaskState
			{
				id = appUninstallTaskState.Id,
				hasErrorField = appUninstallTaskState.Error.HasValue,
				error = (PatchKitAppUninstallTaskError)(appUninstallTaskState.Error.HasValue ? appUninstallTaskState.Error.Value : AppUninstallTaskError.Fatal),
				hasFinished = appUninstallTaskState.HasFinished
			};
			return null;
		}
		catch (Exception)
		{
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string getPatchKitAppUpdateTaskState(int patchKitAppUpdateTaskId, ref PatchKitAppUpdateTaskState result)
	{
		try
		{
			AppUpdateTaskState appUpdateTaskState = _containerAppTasksState.AppUpdateTasks.FirstOrDefault((AppUpdateTaskState x) => x.Id == patchKitAppUpdateTaskId);
			result = new PatchKitAppUpdateTaskState
			{
				id = appUpdateTaskState.Id,
				hasProgressField = appUpdateTaskState.Progress.HasValue,
				progress = (appUpdateTaskState.Progress.HasValue ? new PatchKitAppFilesOperationTaskProgress
				{
					packagesProcessedBytesCount = appUpdateTaskState.Progress.Value.PackagesProcessedBytesCount,
					packagesTotalBytesCount = appUpdateTaskState.Progress.Value.PackagesTotalBytesCount,
					packagesProcessedBytesCountPerSecond = appUpdateTaskState.Progress.Value.PackagesProcessedBytesCountPerSecond,
					filesProcessedBytesCount = appUpdateTaskState.Progress.Value.FilesProcessedBytesCount,
					filesTotalBytesCount = appUpdateTaskState.Progress.Value.FilesTotalBytesCount,
					filesProcessedBytesCountPerSecond = appUpdateTaskState.Progress.Value.FilesProcessedBytesCountPerSecond
				} : default(PatchKitAppFilesOperationTaskProgress)),
				hasErrorField = appUpdateTaskState.Error.HasValue,
				error = (PatchKitAppUpdateTaskError)(appUpdateTaskState.Error.HasValue ? appUpdateTaskState.Error.Value : AppUpdateTaskError.Fatal),
				hasFinished = appUpdateTaskState.HasFinished
			};
			return null;
		}
		catch (Exception)
		{
			return "fatal_error";
		}
	}

	public static void initLibpkappsNet(string libpkappsNetLogsDirPath, OperatingSystemType operatingSystemType, string patchKitApiClientId, string patchKitApiUrl, string patchKitApiCacheUrl, string patchKitKeysApiUrl)
	{
		_libpkappsNetLogsDirPath = libpkappsNetLogsDirPath;
		_operatingSystemType = operatingSystemType;
		_patchKitApiClientId = patchKitApiClientId;
		_patchKitApiUrl = patchKitApiUrl;
		_patchKitApiCacheUrl = patchKitApiCacheUrl;
		_patchKitKeysApiUrl = patchKitKeysApiUrl;
		_isLibpkappsNetInitialized = true;
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string startPatchKitApp([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppProcessExtraArgsAsText, ref StartPatchKitAppResult result)
	{
		using _LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("startPatchKitApp");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App app = new App(path);
			Text? appProcessExtraArgsAsText = ((patchKitAppProcessExtraArgsAsText == null) ? ((Text?)null) : new Text?(new Text(patchKitAppProcessExtraArgsAsText)));
			StartAppDelegate startAppDelegate = libpkappsNetContext.ContainerResolve<StartAppDelegate>();
			ProcessId processId = startAppDelegate(app, appProcessExtraArgsAsText, null, null, null, null);
			result = new StartPatchKitAppResult
			{
				patchKitAppProcessId = processId
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (AppIsNotInstalled)
		{
			return "patchkit_app_is_not_installed";
		}
		catch (AppNeedsRepair)
		{
			return "patchkit_app_needs_repair";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string startPatchKitAppInstallTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppSecret, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppLicenseKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppAuthKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppPassword, ref StartPatchKitAppInstallTaskResult result)
	{
		_LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("patchKitAppInstallTask");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			AppSecret secret = new AppSecret(patchKitAppSecret);
			AppLicenseKey? containerAppLicenseKey = ((patchKitAppLicenseKey == null) ? ((AppLicenseKey?)null) : new AppLicenseKey?(new AppLicenseKey(patchKitAppLicenseKey)));
			AppAuthKey? authKey = ((patchKitAppAuthKey == null) ? ((AppAuthKey?)null) : new AppAuthKey?(new AppAuthKey(patchKitAppAuthKey)));
			AppPassword? containerAppPassword = ((patchKitAppPassword == null) ? ((AppPassword?)null) : new AppPassword?(new AppPassword(patchKitAppPassword)));
			App containerApp = new App(path);
			AppRemote containerAppRemote = new AppRemote(secret, authKey);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			if (!doesAppRootDirExistDelegate(containerApp, null))
			{
				throw new AppRootDirDoesNotExist();
			}
			DoesAppNeedRepairDelegate doesAppNeedRepairDelegate = libpkappsNetContext.ContainerResolve<DoesAppNeedRepairDelegate>();
			if (doesAppNeedRepairDelegate(containerApp, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null))
			{
				throw new AppNeedsRepair();
			}
			GetAppLastFilesOperationTaskStateDelegate getAppLastFilesOperationTaskStateDelegate = libpkappsNetContext.ContainerResolve<GetAppLastFilesOperationTaskStateDelegate>();
			AppLastFilesOperationTaskState? appLastFilesOperationTaskState = getAppLastFilesOperationTaskStateDelegate(containerApp, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null);
			if (appLastFilesOperationTaskState.HasValue)
			{
				AppLastInstallTaskState? appLastInstallTaskState = appLastFilesOperationTaskState.GetValueOrDefault().AppLastInstallTaskState;
				if (appLastInstallTaskState.HasValue && appLastInstallTaskState.GetValueOrDefault().Error.HasValue)
				{
					goto IL_023f;
				}
			}
			if (appLastFilesOperationTaskState.HasValue)
			{
				AppLastUpdateTaskState? appLastUpdateTaskState = appLastFilesOperationTaskState.GetValueOrDefault().AppLastUpdateTaskState;
				if (appLastUpdateTaskState.HasValue && appLastUpdateTaskState.GetValueOrDefault().Error.HasValue)
				{
					goto IL_023f;
				}
			}
			int num = ((appLastFilesOperationTaskState.HasValue && (appLastFilesOperationTaskState.GetValueOrDefault().AppLastRepairTaskState?.Error.HasValue ?? false)) ? 1 : 0);
			goto IL_0240;
			IL_023f:
			num = 1;
			goto IL_0240;
			IL_0240:
			bool containerHasAppLastFilesOperationTaskFailed = (byte)num != 0;
			AppLastInstallTaskState? appLastInstallTaskState2 = appLastFilesOperationTaskState?.AppLastInstallTaskState;
			if (appLastFilesOperationTaskState.HasValue & containerHasAppLastFilesOperationTaskFailed)
			{
				if (!appLastInstallTaskState2.HasValue)
				{
					throw new StartingAppTaskIsForbidden();
				}
			}
			else
			{
				IsAppInstalledDelegate isAppInstalledDelegate = libpkappsNetContext.ContainerResolve<IsAppInstalledDelegate>();
				if (isAppInstalledDelegate(containerApp, (AppRootDirDoesNotExist e) =>
				{
					throw new UnpredictedException(e);
				}, null))
				{
					throw new AppIsAlreadyInstalled();
				}
			}
			AppFilesOperationTaskProgress? containerAppLastInstallTaskStateProgress = appLastInstallTaskState2?.Progress;
			AppInstallTaskState containerAppInstallTaskState = new AppInstallTaskState
			{
				Id = ++_containerAppTasksState.LastAppTaskId,
				CancellationTokenSource = new CancellationTokenSource(),
				Progress = (containerHasAppLastFilesOperationTaskFailed ? containerAppLastInstallTaskStateProgress : ((AppFilesOperationTaskProgress?)null)),
				HasFinished = false
			};
			_containerAppTasksState.AppInstallTasks.Add(containerAppInstallTaskState);
			SetAppLastFilesOperationTaskStateDelegate containerSetAppLastFilesOperationTaskState = libpkappsNetContext.ContainerResolve<SetAppLastFilesOperationTaskStateDelegate>();
			WriteAppMetaDataCacheDelegate containerWriteAppMetaDataCache = libpkappsNetContext.ContainerResolve<WriteAppMetaDataCacheDelegate>();
			ThreadPool.QueueUserWorkItem((object _) =>
			{
				AppFilesOperationTaskProgress? containerAppLastFilesOperationTaskProgress = null;
				try
				{
					UpdateAppDelegate updateAppDelegate = libpkappsNetContext.ContainerResolve<UpdateAppDelegate>();
					updateAppDelegate(containerApp, containerAppRemote, containerAppLicenseKey, containerAppPassword, AppValidationMode.MetaData, (UpdateAppProgress updateAppProgress) =>
					{
						AppFilesOperationTaskProgress value = new AppFilesOperationTaskProgress(updateAppProgress.PackagesProcessedBytesCount, updateAppProgress.PackagesTotalBytesCount, updateAppProgress.PackagesProcessedBytesCountPerSecond, updateAppProgress.FilesProcessedBytesCount, updateAppProgress.FilesTotalBytesCount, updateAppProgress.FilesProcessedBytesCountPerSecond);
						if (containerAppLastInstallTaskStateProgress.HasValue & containerHasAppLastFilesOperationTaskFailed)
						{
							value = value.ModifyPackagesProcessedBytesCount(new BytesCount(value.PackagesProcessedBytesCount + containerAppLastInstallTaskStateProgress.Value.PackagesProcessedBytesCount));
							value = value.ModifyPackagesTotalBytesCount(new BytesCount(value.PackagesTotalBytesCount + containerAppLastInstallTaskStateProgress.Value.PackagesProcessedBytesCount));
						}
						containerAppInstallTaskState.Progress = value;
						containerAppLastFilesOperationTaskProgress = value.ModifyPackagesProcessedBytesCount(new BytesCount((long)value.PackagesProcessedBytesCount - (updateAppProgress.PackagesCurrentFileProcessedBytesCount?.Value ?? 0)));
						containerSetAppLastFilesOperationTaskState(containerApp, new AppLastFilesOperationTaskState(new AppLastInstallTaskState(containerAppLastFilesOperationTaskProgress, AppInstallTaskError.Cancelled), null, null), (AppRootDirDoesNotExist e) =>
						{
						}, (UnauthorizedAccessToAppRootDir e) =>
						{
						});
					}, containerAppInstallTaskState.CancellationTokenSource.Token, (AppRootDirDoesNotExist e) =>
					{
						throw new UnpredictedException(e);
					}, null, null, null, null, null, null, null, null, null, null);
				}
				catch (UnauthorizedAccessToAppRootDir)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.UnauthorizedAccessToAppDir;
				}
				catch (InvalidAppAuthKey)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.InvalidAppAuthKey;
				}
				catch (ExpiredAppAuthKey)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.ExpiredAppAuthKey;
				}
				catch (AppLicenseKeyRequired)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.RequiredAppLicenseKey;
				}
				catch (InvalidAppLicenseKey)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.InvalidAppLicenseKey;
				}
				catch (BlockedAppLicenseKey)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.BlockedAppLicenseKey;
				}
				catch (InvalidAppPassword)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.InvalidAppPassword;
				}
				catch (OutOfAppRootDirFreeSpace)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.OutOfAppDirFreeSpace;
				}
				catch (OperationCanceledException)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.Cancelled;
				}
				catch (FunctionFatalError containerFuncFatalError2)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError2);
				}
				catch (Exception inner2)
				{
					containerAppInstallTaskState.Error = AppInstallTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner2));
				}
				finally
				{
					try
					{
						containerSetAppLastFilesOperationTaskState(containerApp, new AppLastFilesOperationTaskState(new AppLastInstallTaskState(containerAppLastFilesOperationTaskProgress, containerAppInstallTaskState.Error), null, null), (AppRootDirDoesNotExist e) =>
						{
						}, (UnauthorizedAccessToAppRootDir e) =>
						{
						});
						containerWriteAppMetaDataCache(containerApp, (UnauthorizedAccess e) =>
						{
						});
					}
					catch
					{
					}
					containerAppInstallTaskState.HasFinished = true;
					libpkappsNetContext.Dispose();
				}
			});
			result = new StartPatchKitAppInstallTaskResult
			{
				patchKitAppInstallTaskId = containerAppInstallTaskState.Id
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (AppNeedsRepair)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_needs_repair";
		}
		catch (StartingAppTaskIsForbidden)
		{
			libpkappsNetContext.Dispose();
			return "starting_patchkit_app_task_is_forbidden";
		}
		catch (AppIsAlreadyInstalled)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_is_already_installed";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string startPatchKitAppMoveTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppNewDirPath, ref StartPatchKitAppMoveTaskResult result)
	{
		_LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("patchKitAppMoveTask");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			PatchKit.Core.IO.Path containerAppNewRootDirPath = new PatchKit.Core.IO.Path(patchKitAppNewDirPath);
			if (containerAppNewRootDirPath.Value.Contains(path.Value + "/"))
			{
				throw new AppNewRootDirIsNotEmpty();
			}
			App containerApp = new App(path);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			if (!doesAppRootDirExistDelegate(containerApp, null))
			{
				throw new AppRootDirDoesNotExist();
			}
			CreateDirDelegate createDirDelegate = libpkappsNetContext.ContainerResolve<CreateDirDelegate>();
			createDirDelegate(containerAppNewRootDirPath, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppNewRootDir(e);
			}, (ConflictingFile e) =>
			{
				throw new FileIsConflictingWithAppNewRootDir(e);
			});
			IsDirEmptyDelegate isDirEmptyDelegate = libpkappsNetContext.ContainerResolve<IsDirEmptyDelegate>();
			if (!isDirEmptyDelegate(containerAppNewRootDirPath, (UnauthorizedAccess e) =>
			{
				throw new UnauthorizedAccessToAppNewRootDir(e);
			}, (NotExistingDir e) =>
			{
				throw new UnpredictedException(e);
			}))
			{
				throw new AppNewRootDirIsNotEmpty();
			}
			AppMoveTaskState containerAppMoveTaskState = new AppMoveTaskState
			{
				Id = ++_containerAppTasksState.LastAppTaskId,
				HasFinished = false
			};
			_containerAppTasksState.AppMoveTasks.Add(containerAppMoveTaskState);
			ThreadPool.QueueUserWorkItem((object _) =>
			{
				try
				{
					MoveAppDelegate moveAppDelegate = libpkappsNetContext.ContainerResolve<MoveAppDelegate>();
					moveAppDelegate(containerApp, containerAppNewRootDirPath, (AppNewRootDirIsNotEmpty e) =>
					{
						throw new UnpredictedException(e);
					}, null, null, (FileIsConflictingWithAppNewRootDir e) =>
					{
						throw new UnpredictedException(e);
					}, null);
				}
				catch (UnauthorizedAccessToAppRootDir)
				{
					containerAppMoveTaskState.Error = AppMoveTaskError.UnauthorizedAccessToAppDir;
				}
				catch (UnauthorizedAccessToAppNewRootDir)
				{
					containerAppMoveTaskState.Error = AppMoveTaskError.UnauthorizedAccessToAppNewDir;
				}
				catch (OutOfAppNewRootDirFreeSpace)
				{
					containerAppMoveTaskState.Error = AppMoveTaskError.OutOfAppDirFreeSpace;
				}
				catch (OperationCanceledException)
				{
					containerAppMoveTaskState.Error = AppMoveTaskError.Cancelled;
				}
				catch (FunctionFatalError containerFuncFatalError2)
				{
					containerAppMoveTaskState.Error = AppMoveTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError2);
				}
				catch (Exception inner2)
				{
					containerAppMoveTaskState.Error = AppMoveTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner2));
				}
				finally
				{
					containerAppMoveTaskState.HasFinished = true;
					libpkappsNetContext.Dispose();
				}
			});
			result = new StartPatchKitAppMoveTaskResult
			{
				patchKitAppMoveTaskId = containerAppMoveTaskState.Id
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (UnauthorizedAccessToAppNewRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_new_dir";
		}
		catch (FileIsConflictingWithAppNewRootDir)
		{
			libpkappsNetContext.Dispose();
			return "file_is_conflicting_with_patchkit_app_new_dir";
		}
		catch (AppNewRootDirIsNotEmpty)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_new_dir_is_not_empty";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string startPatchKitAppRepairTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppSecret, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppLicenseKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppAuthKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppPassword, ref StartPatchKitAppRepairTaskResult result)
	{
		_LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("patchKitAppRepairTask");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			AppSecret secret = new AppSecret(patchKitAppSecret);
			AppLicenseKey? containerAppLicenseKey = ((patchKitAppLicenseKey == null) ? ((AppLicenseKey?)null) : new AppLicenseKey?(new AppLicenseKey(patchKitAppLicenseKey)));
			AppAuthKey? authKey = ((patchKitAppAuthKey == null) ? ((AppAuthKey?)null) : new AppAuthKey?(new AppAuthKey(patchKitAppAuthKey)));
			AppPassword? containerAppPassword = ((patchKitAppPassword == null) ? ((AppPassword?)null) : new AppPassword?(new AppPassword(patchKitAppPassword)));
			App containerApp = new App(path);
			AppRemote containerAppRemote = new AppRemote(secret, authKey);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			if (!doesAppRootDirExistDelegate(containerApp, null))
			{
				throw new AppRootDirDoesNotExist();
			}
			GetAppLastFilesOperationTaskStateDelegate getAppLastFilesOperationTaskStateDelegate = libpkappsNetContext.ContainerResolve<GetAppLastFilesOperationTaskStateDelegate>();
			AppLastFilesOperationTaskState? appLastFilesOperationTaskState = getAppLastFilesOperationTaskStateDelegate(containerApp, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null);
			if (appLastFilesOperationTaskState.HasValue)
			{
				AppLastInstallTaskState? appLastInstallTaskState = appLastFilesOperationTaskState.GetValueOrDefault().AppLastInstallTaskState;
				if (appLastInstallTaskState.HasValue && appLastInstallTaskState.GetValueOrDefault().Error.HasValue)
				{
					goto IL_01f2;
				}
			}
			if (appLastFilesOperationTaskState.HasValue)
			{
				AppLastUpdateTaskState? appLastUpdateTaskState = appLastFilesOperationTaskState.GetValueOrDefault().AppLastUpdateTaskState;
				if (appLastUpdateTaskState.HasValue && appLastUpdateTaskState.GetValueOrDefault().Error.HasValue)
				{
					goto IL_01f2;
				}
			}
			int num = ((appLastFilesOperationTaskState.HasValue && (appLastFilesOperationTaskState.GetValueOrDefault().AppLastRepairTaskState?.Error.HasValue ?? false)) ? 1 : 0);
			goto IL_01f3;
			IL_01f3:
			bool containerHasAppLastFilesOperationTaskFailed = (byte)num != 0;
			AppFilesOperationTaskProgress? containerAppLastRepairTaskStateProgress = (appLastFilesOperationTaskState?.AppLastRepairTaskState)?.Progress;
			AppRepairTaskState containerAppRepairTaskState = new AppRepairTaskState
			{
				Id = ++_containerAppTasksState.LastAppTaskId,
				CancellationTokenSource = new CancellationTokenSource(),
				Progress = (containerHasAppLastFilesOperationTaskFailed ? containerAppLastRepairTaskStateProgress : ((AppFilesOperationTaskProgress?)null)),
				HasFinished = false
			};
			_containerAppTasksState.AppRepairTasks.Add(containerAppRepairTaskState);
			SetAppLastFilesOperationTaskStateDelegate containerSetAppLastFilesOperationTaskState = libpkappsNetContext.ContainerResolve<SetAppLastFilesOperationTaskStateDelegate>();
			WriteAppMetaDataCacheDelegate containerWriteAppMetaDataCache = libpkappsNetContext.ContainerResolve<WriteAppMetaDataCacheDelegate>();
			ThreadPool.QueueUserWorkItem((object _) =>
			{
				AppFilesOperationTaskProgress? containerAppLastFilesOperationTaskProgress = null;
				try
				{
					UpdateAppDelegate updateAppDelegate = libpkappsNetContext.ContainerResolve<UpdateAppDelegate>();
					updateAppDelegate(containerApp, containerAppRemote, containerAppLicenseKey, containerAppPassword, AppValidationMode.Hashes, (UpdateAppProgress updateAppProgress) =>
					{
						AppFilesOperationTaskProgress value = new AppFilesOperationTaskProgress(updateAppProgress.PackagesProcessedBytesCount, updateAppProgress.PackagesTotalBytesCount, updateAppProgress.PackagesProcessedBytesCountPerSecond, updateAppProgress.FilesProcessedBytesCount, updateAppProgress.FilesTotalBytesCount, updateAppProgress.FilesProcessedBytesCountPerSecond);
						if (containerAppLastRepairTaskStateProgress.HasValue & containerHasAppLastFilesOperationTaskFailed)
						{
							value = value.ModifyPackagesProcessedBytesCount(new BytesCount(value.PackagesProcessedBytesCount + containerAppLastRepairTaskStateProgress.Value.PackagesProcessedBytesCount));
							value = value.ModifyPackagesTotalBytesCount(new BytesCount(value.PackagesTotalBytesCount + containerAppLastRepairTaskStateProgress.Value.PackagesProcessedBytesCount));
						}
						containerAppRepairTaskState.Progress = value;
						containerAppLastFilesOperationTaskProgress = value.ModifyPackagesProcessedBytesCount(new BytesCount((long)value.PackagesProcessedBytesCount - (updateAppProgress.PackagesCurrentFileProcessedBytesCount?.Value ?? 0)));
						containerSetAppLastFilesOperationTaskState(containerApp, new AppLastFilesOperationTaskState(null, null, new AppLastRepairTaskState(containerAppLastFilesOperationTaskProgress, AppRepairTaskError.Cancelled)), (AppRootDirDoesNotExist e) =>
						{
						}, (UnauthorizedAccessToAppRootDir e) =>
						{
						});
					}, containerAppRepairTaskState.CancellationTokenSource.Token, (AppRootDirDoesNotExist e) =>
					{
						throw new UnpredictedException(e);
					}, null, null, null, null, null, null, null, null, null, null);
				}
				catch (UnauthorizedAccessToAppRootDir)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.UnauthorizedAccessToAppDir;
				}
				catch (InvalidAppAuthKey)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.InvalidAppAuthKey;
				}
				catch (ExpiredAppAuthKey)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.ExpiredAppAuthKey;
				}
				catch (AppLicenseKeyRequired)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.RequiredAppLicenseKey;
				}
				catch (InvalidAppLicenseKey)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.InvalidAppLicenseKey;
				}
				catch (BlockedAppLicenseKey)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.BlockedAppLicenseKey;
				}
				catch (InvalidAppPassword)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.InvalidAppPassword;
				}
				catch (OutOfAppRootDirFreeSpace)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.OutOfAppDirFreeSpace;
				}
				catch (OperationCanceledException)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.Cancelled;
				}
				catch (FunctionFatalError containerFuncFatalError2)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError2);
				}
				catch (Exception inner2)
				{
					containerAppRepairTaskState.Error = AppRepairTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner2));
				}
				finally
				{
					try
					{
						containerSetAppLastFilesOperationTaskState(containerApp, new AppLastFilesOperationTaskState(null, null, new AppLastRepairTaskState(containerAppLastFilesOperationTaskProgress, containerAppRepairTaskState.Error)), (AppRootDirDoesNotExist e) =>
						{
						}, (UnauthorizedAccessToAppRootDir e) =>
						{
						});
						containerWriteAppMetaDataCache(containerApp, (UnauthorizedAccess e) =>
						{
						});
					}
					catch
					{
					}
					containerAppRepairTaskState.HasFinished = true;
					libpkappsNetContext.Dispose();
				}
			});
			result = new StartPatchKitAppRepairTaskResult
			{
				patchKitAppRepairTaskId = containerAppRepairTaskState.Id
			};
			return null;
			IL_01f2:
			num = 1;
			goto IL_01f3;
		}
		catch (AppRootDirDoesNotExist)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string startPatchKitAppUninstallTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, ref StartPatchKitAppUninstallTaskResult result)
	{
		_LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("patchKitAppUninstallTask");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			App containerApp = new App(path);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			if (!doesAppRootDirExistDelegate(containerApp, null))
			{
				throw new AppRootDirDoesNotExist();
			}
			AppUninstallTaskState containerAppUninstallTaskState = new AppUninstallTaskState
			{
				Id = ++_containerAppTasksState.LastAppTaskId,
				HasFinished = false
			};
			_containerAppTasksState.AppUninstallTasks.Add(containerAppUninstallTaskState);
			ThreadPool.QueueUserWorkItem((object _) =>
			{
				try
				{
					UninstallAppDelegate uninstallAppDelegate = libpkappsNetContext.ContainerResolve<UninstallAppDelegate>();
					uninstallAppDelegate(containerApp, (AppRootDirDoesNotExist e) =>
					{
						throw new UnpredictedException(e);
					}, null);
				}
				catch (UnauthorizedAccessToAppRootDir)
				{
					containerAppUninstallTaskState.Error = AppUninstallTaskError.UnauthorizedAccessToAppDir;
				}
				catch (OperationCanceledException)
				{
					containerAppUninstallTaskState.Error = AppUninstallTaskError.Cancelled;
				}
				catch (FunctionFatalError containerFuncFatalError2)
				{
					containerAppUninstallTaskState.Error = AppUninstallTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError2);
				}
				catch (Exception inner2)
				{
					containerAppUninstallTaskState.Error = AppUninstallTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner2));
				}
				finally
				{
					containerAppUninstallTaskState.HasFinished = true;
					libpkappsNetContext.Dispose();
				}
			});
			result = new StartPatchKitAppUninstallTaskResult
			{
				patchKitAppUninstallTaskId = containerAppUninstallTaskState.Id
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	[return: MarshalAs(UnmanagedType.LPUTF8Str)]
	public static string startPatchKitAppUpdateTask([MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppDirPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppSecret, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppLicenseKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppAuthKey, [MarshalAs(UnmanagedType.LPUTF8Str)] string patchKitAppPassword, ref StartPatchKitAppUpdateTaskResult result)
	{
		_LibpkappsNetContext libpkappsNetContext = new _LibpkappsNetContext("patchKitAppUpdateTask");
		try
		{
			PatchKit.Core.IO.Path path = new PatchKit.Core.IO.Path(patchKitAppDirPath);
			AppSecret secret = new AppSecret(patchKitAppSecret);
			AppLicenseKey? containerAppLicenseKey = ((patchKitAppLicenseKey == null) ? ((AppLicenseKey?)null) : new AppLicenseKey?(new AppLicenseKey(patchKitAppLicenseKey)));
			AppAuthKey? authKey = ((patchKitAppAuthKey == null) ? ((AppAuthKey?)null) : new AppAuthKey?(new AppAuthKey(patchKitAppAuthKey)));
			AppPassword? containerAppPassword = ((patchKitAppPassword == null) ? ((AppPassword?)null) : new AppPassword?(new AppPassword(patchKitAppPassword)));
			App containerApp = new App(path);
			AppRemote containerAppRemote = new AppRemote(secret, authKey);
			DoesAppRootDirExistDelegate doesAppRootDirExistDelegate = libpkappsNetContext.ContainerResolve<DoesAppRootDirExistDelegate>();
			if (!doesAppRootDirExistDelegate(containerApp, null))
			{
				throw new AppRootDirDoesNotExist();
			}
			DoesAppNeedRepairDelegate doesAppNeedRepairDelegate = libpkappsNetContext.ContainerResolve<DoesAppNeedRepairDelegate>();
			if (doesAppNeedRepairDelegate(containerApp, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null))
			{
				throw new AppNeedsRepair();
			}
			GetAppLastFilesOperationTaskStateDelegate getAppLastFilesOperationTaskStateDelegate = libpkappsNetContext.ContainerResolve<GetAppLastFilesOperationTaskStateDelegate>();
			AppLastFilesOperationTaskState? appLastFilesOperationTaskState = getAppLastFilesOperationTaskStateDelegate(containerApp, (AppRootDirDoesNotExist e) =>
			{
				throw new UnpredictedException(e);
			}, null);
			if (appLastFilesOperationTaskState.HasValue)
			{
				AppLastInstallTaskState? appLastInstallTaskState = appLastFilesOperationTaskState.GetValueOrDefault().AppLastInstallTaskState;
				if (appLastInstallTaskState.HasValue && appLastInstallTaskState.GetValueOrDefault().Error.HasValue)
				{
					goto IL_023f;
				}
			}
			if (appLastFilesOperationTaskState.HasValue)
			{
				AppLastUpdateTaskState? appLastUpdateTaskState = appLastFilesOperationTaskState.GetValueOrDefault().AppLastUpdateTaskState;
				if (appLastUpdateTaskState.HasValue && appLastUpdateTaskState.GetValueOrDefault().Error.HasValue)
				{
					goto IL_023f;
				}
			}
			int num = ((appLastFilesOperationTaskState.HasValue && (appLastFilesOperationTaskState.GetValueOrDefault().AppLastRepairTaskState?.Error.HasValue ?? false)) ? 1 : 0);
			goto IL_0240;
			IL_023f:
			num = 1;
			goto IL_0240;
			IL_0240:
			bool containerHasAppLastFilesOperationTaskFailed = (byte)num != 0;
			AppFilesOperationTaskProgress? containerAppLastUpdateTaskStateProgress = (appLastFilesOperationTaskState?.AppLastUpdateTaskState)?.Progress;
			AppUpdateTaskState containerAppUpdateTaskState = new AppUpdateTaskState
			{
				Id = ++_containerAppTasksState.LastAppTaskId,
				CancellationTokenSource = new CancellationTokenSource(),
				Progress = (containerHasAppLastFilesOperationTaskFailed ? containerAppLastUpdateTaskStateProgress : ((AppFilesOperationTaskProgress?)null)),
				HasFinished = false
			};
			_containerAppTasksState.AppUpdateTasks.Add(containerAppUpdateTaskState);
			SetAppLastFilesOperationTaskStateDelegate containerSetAppLastFilesOperationTaskState = libpkappsNetContext.ContainerResolve<SetAppLastFilesOperationTaskStateDelegate>();
			WriteAppMetaDataCacheDelegate containerWriteAppMetaDataCache = libpkappsNetContext.ContainerResolve<WriteAppMetaDataCacheDelegate>();
			ThreadPool.QueueUserWorkItem((object _) =>
			{
				AppFilesOperationTaskProgress? containerAppLastFilesOperationTaskProgress = null;
				try
				{
					UpdateAppDelegate updateAppDelegate = libpkappsNetContext.ContainerResolve<UpdateAppDelegate>();
					updateAppDelegate(containerApp, containerAppRemote, containerAppLicenseKey, containerAppPassword, AppValidationMode.MetaData, (UpdateAppProgress updateAppProgress) =>
					{
						AppFilesOperationTaskProgress value = new AppFilesOperationTaskProgress(updateAppProgress.PackagesProcessedBytesCount, updateAppProgress.PackagesTotalBytesCount, updateAppProgress.PackagesProcessedBytesCountPerSecond, updateAppProgress.FilesProcessedBytesCount, updateAppProgress.FilesTotalBytesCount, updateAppProgress.FilesProcessedBytesCountPerSecond);
						if (containerAppLastUpdateTaskStateProgress.HasValue & containerHasAppLastFilesOperationTaskFailed)
						{
							value = value.ModifyPackagesProcessedBytesCount(new BytesCount(value.PackagesProcessedBytesCount + containerAppLastUpdateTaskStateProgress.Value.PackagesProcessedBytesCount));
							value = value.ModifyPackagesTotalBytesCount(new BytesCount(value.PackagesTotalBytesCount + containerAppLastUpdateTaskStateProgress.Value.PackagesProcessedBytesCount));
						}
						containerAppUpdateTaskState.Progress = value;
						containerAppLastFilesOperationTaskProgress = value.ModifyPackagesProcessedBytesCount(new BytesCount((long)value.PackagesProcessedBytesCount - (updateAppProgress.PackagesCurrentFileProcessedBytesCount?.Value ?? 0)));
						containerSetAppLastFilesOperationTaskState(containerApp, new AppLastFilesOperationTaskState(null, new AppLastUpdateTaskState(containerAppLastFilesOperationTaskProgress, AppUpdateTaskError.Cancelled), null), (AppRootDirDoesNotExist e) =>
						{
						}, (UnauthorizedAccessToAppRootDir e) =>
						{
						});
					}, containerAppUpdateTaskState.CancellationTokenSource.Token, (AppRootDirDoesNotExist e) =>
					{
						throw new UnpredictedException(e);
					}, null, null, null, null, null, null, null, null, null, null);
				}
				catch (UnauthorizedAccessToAppRootDir)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.UnauthorizedAccessToAppDir;
				}
				catch (InvalidAppAuthKey)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.InvalidAppAuthKey;
				}
				catch (ExpiredAppAuthKey)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.ExpiredAppAuthKey;
				}
				catch (AppLicenseKeyRequired)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.RequiredAppLicenseKey;
				}
				catch (InvalidAppLicenseKey)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.InvalidAppLicenseKey;
				}
				catch (BlockedAppLicenseKey)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.BlockedAppLicenseKey;
				}
				catch (InvalidAppPassword)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.InvalidAppPassword;
				}
				catch (OutOfAppRootDirFreeSpace)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.OutOfAppDirFreeSpace;
				}
				catch (OperationCanceledException)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.Cancelled;
				}
				catch (FunctionFatalError containerFuncFatalError2)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError2);
				}
				catch (Exception inner2)
				{
					containerAppUpdateTaskState.Error = AppUpdateTaskError.Fatal;
					libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner2));
				}
				finally
				{
					try
					{
						containerSetAppLastFilesOperationTaskState(containerApp, new AppLastFilesOperationTaskState(null, new AppLastUpdateTaskState(containerAppLastFilesOperationTaskProgress, containerAppUpdateTaskState.Error), null), (AppRootDirDoesNotExist e) =>
						{
						}, (UnauthorizedAccessToAppRootDir e) =>
						{
						});
						containerWriteAppMetaDataCache(containerApp, (UnauthorizedAccess e) =>
						{
						});
					}
					catch
					{
					}
					containerAppUpdateTaskState.HasFinished = true;
					libpkappsNetContext.Dispose();
				}
			});
			result = new StartPatchKitAppUpdateTaskResult
			{
				patchKitAppUpdateTaskId = containerAppUpdateTaskState.Id
			};
			return null;
		}
		catch (AppRootDirDoesNotExist)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_dir_does_not_exist";
		}
		catch (UnauthorizedAccessToAppRootDir)
		{
			libpkappsNetContext.Dispose();
			return "unauthorized_access_to_patchkit_app_dir";
		}
		catch (AppNeedsRepair)
		{
			libpkappsNetContext.Dispose();
			return "patchkit_app_needs_repair";
		}
		catch (FunctionFatalError containerFuncFatalError)
		{
			libpkappsNetContext.SetContainerFuncFatalError(containerFuncFatalError);
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
		catch (Exception inner)
		{
			libpkappsNetContext.SetContainerFuncFatalError(new FunctionFatalError(inner));
			libpkappsNetContext.Dispose();
			return "fatal_error";
		}
	}

	private static SentryClient _getLibpkappsNetSentryClient()
	{
		return new SentryClient(new SentryOptions
		{
			Dsn = new Dsn("https://cab98399a70a4b7e948464152102ed8c@sentry.io/1432868"),
			Environment = "development",
			Release = "libpkapps@" + _LibpkappsNetVersion.Text
		});
	}

	private static string _predictLibpkappsNetUploadedLogUrl(string libpkappsNetUploadedLogGuid)
	{
		DateTime utcNow = DateTime.UtcNow;
		string text = $"{utcNow.Year}_{utcNow.Month:D2}_{utcNow.Day:D2}";
		return "https://patchkit-app-logs.s3.us-west-2.amazonaws.com/libpkapps/" + text + "/" + libpkappsNetUploadedLogGuid + ".201-log.gz";
	}

	private static async Task<string> _uploadLibpkappsNetLogAsync(string libpkappsNetLogFilePath, string libpkappsNetLogGuid)
	{
		try
		{
			string putUrl = await RequestPutUrl(libpkappsNetLogGuid, "libpkapps", _LibpkappsNetVersion.Text);
			byte[] compressedLog = GetCompressedLog(libpkappsNetLogFilePath);
			if (!(await SendCompressedLog(putUrl, compressedLog)))
			{
				return null;
			}
			return _predictLibpkappsNetUploadedLogUrl(libpkappsNetLogGuid);
		}
		catch
		{
			return null;
		}
	}

	private static async Task<bool> SendCompressedLog(string putUrl, byte[] compressedLog)
	{
		using HttpClient client = new HttpClient();
		using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, new Uri(putUrl));
		using ByteArrayContent byteArrayContent = new ByteArrayContent(compressedLog);
		request.Content = byteArrayContent;
		using HttpResponseMessage response = await client.SendAsync(request);
		return response.IsSuccessStatusCode;
	}

	private static byte[] GetCompressedLog(string path)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (SharpCompress.Compressors.Deflate.GZipStream destination = new SharpCompress.Compressors.Deflate.GZipStream(memoryStream, SharpCompress.Compressors.CompressionMode.Compress))
		{
			using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			fileStream.CopyTo(destination);
		}
		return memoryStream.ToArray();
	}

	private static async Task<string> RequestPutUrl(string guid, string appId, string appVersion)
	{
		RequestPutUrlContent content = new RequestPutUrlContent
		{
			AppId = appId,
			Version = appVersion,
			Priority = "201",
			Guid = guid,
			Compression = "gz"
		};
		string contentJson = JsonConvert.SerializeObject(content);
		using HttpClient client = new HttpClient();
		using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://se5ia30ji3.execute-api.us-west-2.amazonaws.com/production/v1/request-put-url"));
		using StringContent stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
		request.Content = stringContent;
		using HttpResponseMessage response = await client.SendAsync(request);
		return JsonConvert.DeserializeObject<RequestPutUrlResponseData>(await response.Content.ReadAsStringAsync()).Url;
	}
}
