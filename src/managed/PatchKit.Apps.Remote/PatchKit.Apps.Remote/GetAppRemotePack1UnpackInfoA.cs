using System;
using System.ComponentModel;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public class GetAppRemotePack1UnpackInfoA
{
	private struct VersionData
	{
		[JsonProperty("version")]
		public string Version;
	}

	private struct Data_1_x_x
	{
		public struct File
		{
			public const string RegularType = "regular";

			[JsonProperty("name")]
			public string Name;

			[JsonProperty("type")]
			public string Type;

			[JsonProperty("target")]
			public string Target;

			[JsonProperty("mode")]
			public string Mode;

			[JsonProperty("offset")]
			public long Offset;

			[JsonProperty("size")]
			public long Size;
		}

		public const string AesEncryption = "aes";

		public const string GzipCompression = "gzip";

		public const string XzCompression = "xz";

		[JsonProperty("version")]
		public string Version;

		[DefaultValue("aes")]
		[JsonProperty("encryptionType", DefaultValueHandling = DefaultValueHandling.Populate)]
		public string Encryption;

		[JsonProperty("compression", DefaultValueHandling = DefaultValueHandling.Populate)]
		[DefaultValue("gzip")]
		public string Compression;

		[JsonProperty("iv")]
		public string Iv;

		[JsonProperty("files")]
		public File[] Files;
	}

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePack1UnpackInfo Execute(Text metaText, AppRemotePack1UnpackPassword password)
	{
		bool v = JsonConvert.DeserializeObject<VersionData>(metaText).Version.StartsWith("1.");
		Assert.IsTrue(v, "isFirstVersion");
		Data_1_x_x v2 = JsonConvert.DeserializeObject<Data_1_x_x>(metaText, new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore
		});
		Assert.IsNotNull(v2);
		ImmutableArray<AppRemotePack1FileUnpackInfo> filesInfo = (from x in v2.Files
			where x.Type == "regular"
			select new AppRemotePack1FileUnpackInfo(new AppPath(x.Name), new BytesCount(x.Offset), new BytesCount(x.Size))).AsImmutable();
		bool v3 = v2.Encryption == "aes";
		Assert.IsTrue(v3, "isAesEncryption");
		AppRemotePack1CompressionMethod? v4 = null;
		if (v2.Compression == "gzip")
		{
			v4 = AppRemotePack1CompressionMethod.Gzip;
		}
		else if (v2.Compression == "xz")
		{
			v4 = AppRemotePack1CompressionMethod.Lzma2;
		}
		Assert.IsNotNull(v4);
		bool v5 = string.IsNullOrEmpty(v2.Iv);
		Assert.IsFalse(v5, "isIvNullOrEmpty");
		AppRemotePack1AesEncryption value = new AppRemotePack1AesEncryption(new Text(v2.Iv));
		AppRemotePack1Encryption encryption = new AppRemotePack1Encryption(value);
		AppRemotePack1CompressionMethod value2 = v4.Value;
		return new AppRemotePack1UnpackInfo(filesInfo, encryption, value2, password);
	}

	public GetAppRemotePack1UnpackInfoA(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePack1UnpackInfo DelegateExecute(Text metaText, AppRemotePack1UnpackPassword password)
	{
		Arg.IsValid(metaText, "metaText");
		Arg.IsValid(password, "password");
		LogInfo("Executing GetAppRemotePack1UnpackInfoA...");
		LogTrace("metaText = " + metaText);
		LogTrace("password = " + password);
		try
		{
			BeginLogRegion(enabled: false);
			AppRemotePack1UnpackInfo appRemotePack1UnpackInfo = Execute(metaText, password);
			EndLogRegion(enabled: false);
			AppRemotePack1UnpackInfo appRemotePack1UnpackInfo2 = appRemotePack1UnpackInfo;
			LogTrace("result = " + appRemotePack1UnpackInfo2.ToString());
			LogInfo("GetAppRemotePack1UnpackInfoA finished: success.");
			return appRemotePack1UnpackInfo;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePack1UnpackInfoA has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePack1UnpackInfoA failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePack1UnpackInfoA finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemotePack1UnpackInfoADelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemotePack1UnpackInfoA(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
