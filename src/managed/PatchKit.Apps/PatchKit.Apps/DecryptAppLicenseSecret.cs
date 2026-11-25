using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class DecryptAppLicenseSecret
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

	private AppLicenseSecret? Execute(AppEncryptedLicenseSecret encryptedLicenseSecret, AppLicenseKey licenseKey)
	{
		try
		{
			return UnsafeExecute(encryptedLicenseSecret, licenseKey);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private AppLicenseSecret? UnsafeExecute(AppEncryptedLicenseSecret encryptedLicenseSecret, AppLicenseKey licenseKey)
	{
		byte[] array = Convert.FromBase64String(encryptedLicenseSecret.Value);
		using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(licenseKey.Value, KeyHashSalt, 1024);
		byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.BlockSize = 128;
		rijndaelManaged.Mode = CipherMode.CBC;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, EncodingIv);
		using MemoryStream memoryStream = new MemoryStream(array);
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
		byte[] array2 = new byte[array.Length];
		int count = cryptoStream.Read(array2, 0, array2.Length);
		memoryStream.Close();
		cryptoStream.Close();
		return new AppLicenseSecret(Encoding.UTF8.GetString(array2, 0, count));
	}

	public DecryptAppLicenseSecret(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppLicenseSecret? DelegateExecute(AppEncryptedLicenseSecret encryptedLicenseSecret, AppLicenseKey licenseKey)
	{
		Arg.IsValid(encryptedLicenseSecret, "encryptedLicenseSecret");
		Arg.IsValid(licenseKey, "licenseKey");
		LogInfo("Executing DecryptAppLicenseSecret...");
		LogTrace("encryptedLicenseSecret = " + encryptedLicenseSecret);
		LogTrace("licenseKey = " + licenseKey);
		try
		{
			BeginLogRegion(enabled: true);
			AppLicenseSecret? appLicenseSecret = Execute(encryptedLicenseSecret, licenseKey);
			EndLogRegion(enabled: true);
			AppLicenseSecret? appLicenseSecret2 = appLicenseSecret;
			LogTrace("result = " + (appLicenseSecret2.HasValue ? ((string)appLicenseSecret2.GetValueOrDefault()) : null));
			LogInfo("DecryptAppLicenseSecret finished: success.");
			return appLicenseSecret;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DecryptAppLicenseSecret has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DecryptAppLicenseSecret failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DecryptAppLicenseSecret finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DecryptAppLicenseSecretDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DecryptAppLicenseSecret(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
