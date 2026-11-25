using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class DecryptAppEncryptedPassword
{
	private const int KeySize = 256;

	private const int KeyHashIterations = 1024;

	private static readonly byte[] KeyHashSalt = new byte[32]
	{
		161, 61, 63, 96, 98, 133, 83, 167, 21, 150,
		83, 44, 91, 240, 94, 179, 18, 246, 167, 38,
		80, 60, 120, 49, 15, 111, 176, 19, 61, 39,
		211, 61
	};

	private static readonly byte[] EncodingIv = new byte[16]
	{
		117, 138, 30, 179, 174, 25, 163, 48, 235, 230,
		131, 234, 104, 25, 129, 245
	};

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppPassword? Execute(AppEncryptedPassword appEncryptedPassword, AppSecret appSecret)
	{
		try
		{
			return UnsafeExecute(appEncryptedPassword, appSecret);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private AppPassword? UnsafeExecute(AppEncryptedPassword appEncryptedPassword, AppSecret appSecret)
	{
		byte[] array = Convert.FromBase64String(appEncryptedPassword.Value);
		using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(appSecret.Value, KeyHashSalt, 1024);
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
		return new AppPassword(Encoding.UTF8.GetString(array2, 0, count));
	}

	public DecryptAppEncryptedPassword(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppPassword? DelegateExecute(AppEncryptedPassword appEncryptedPassword, AppSecret appSecret)
	{
		Arg.IsValid(appEncryptedPassword, "appEncryptedPassword");
		Arg.IsValid(appSecret, "appSecret");
		LogInfo("Executing DecryptAppEncryptedPassword...");
		LogTrace("appEncryptedPassword = " + appEncryptedPassword);
		LogTrace("appSecret = " + appSecret);
		try
		{
			BeginLogRegion(enabled: true);
			AppPassword? appPassword = Execute(appEncryptedPassword, appSecret);
			EndLogRegion(enabled: true);
			AppPassword? appPassword2 = appPassword;
			LogTrace("result = " + (appPassword2.HasValue ? ((string)appPassword2.GetValueOrDefault()) : null));
			LogInfo("DecryptAppEncryptedPassword finished: success.");
			return appPassword;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("DecryptAppEncryptedPassword has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("DecryptAppEncryptedPassword failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("DecryptAppEncryptedPassword finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public DecryptAppEncryptedPasswordDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new DecryptAppEncryptedPassword(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
