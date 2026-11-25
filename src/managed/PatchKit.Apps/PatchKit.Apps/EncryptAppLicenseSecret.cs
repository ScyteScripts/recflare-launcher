using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class EncryptAppLicenseSecret
{
	private const int KeySize = 256;

	private const int KeyHashIterations = 1024;

	private static readonly byte[] KeyHashSalt = new byte[32]
	{
		213, 143, 96, 91, 223, 139, 235, 68, 33, 208,
		160, 100, 24, 185, 63, 18, 202, 82, 142, 112,
		171, 226, 245, 153, 114, 76, 26, 216, 60, 179,
		83, 151
	};

	private static readonly byte[] EncodingIv = new byte[16]
	{
		238, 194, 16, 74, 31, 139, 227, 111, 108, 84,
		229, 50, 196, 27, 105, 91
	};

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppEncryptedLicenseSecret Execute(AppLicenseSecret licenseSecret, AppLicenseKey licenseKey)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(licenseSecret.Value);
		using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(licenseKey.Value, KeyHashSalt, 1024);
		byte[] bytes2 = rfc2898DeriveBytes.GetBytes(32);
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.BlockSize = 128;
		rijndaelManaged.Mode = CipherMode.CBC;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		using ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes2, EncodingIv);
		using MemoryStream memoryStream = new MemoryStream();
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.FlushFinalBlock();
		byte[] inArray = memoryStream.ToArray();
		memoryStream.Close();
		cryptoStream.Close();
		return new AppEncryptedLicenseSecret(Convert.ToBase64String(inArray));
	}

	public EncryptAppLicenseSecret(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppEncryptedLicenseSecret DelegateExecute(AppLicenseSecret licenseSecret, AppLicenseKey licenseKey)
	{
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(licenseKey, "licenseKey");
		LogInfo("Executing EncryptAppLicenseSecret...");
		LogTrace("licenseSecret = " + licenseSecret);
		LogTrace("licenseKey = " + licenseKey);
		try
		{
			BeginLogRegion(enabled: true);
			AppEncryptedLicenseSecret appEncryptedLicenseSecret = Execute(licenseSecret, licenseKey);
			EndLogRegion(enabled: true);
			LogTrace("result = " + appEncryptedLicenseSecret);
			LogInfo("EncryptAppLicenseSecret finished: success.");
			return appEncryptedLicenseSecret;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("EncryptAppLicenseSecret has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("EncryptAppLicenseSecret failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("EncryptAppLicenseSecret finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public EncryptAppLicenseSecretDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new EncryptAppLicenseSecret(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
