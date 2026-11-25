using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class EncryptAppPassword
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

	private AppEncryptedPassword Execute(AppPassword appPassword, AppSecret appSecret)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(appPassword.Value);
		using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(appSecret.Value, KeyHashSalt, 1024);
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
		return new AppEncryptedPassword(Convert.ToBase64String(inArray));
	}

	public EncryptAppPassword(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppEncryptedPassword DelegateExecute(AppPassword appPassword, AppSecret appSecret)
	{
		Arg.IsValid(appPassword, "appPassword");
		Arg.IsValid(appSecret, "appSecret");
		LogInfo("Executing EncryptAppPassword...");
		LogTrace("appPassword = " + appPassword);
		LogTrace("appSecret = " + appSecret);
		try
		{
			BeginLogRegion(enabled: true);
			AppEncryptedPassword appEncryptedPassword = Execute(appPassword, appSecret);
			EndLogRegion(enabled: true);
			LogTrace("result = " + appEncryptedPassword);
			LogInfo("EncryptAppPassword finished: success.");
			return appEncryptedPassword;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("EncryptAppPassword has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("EncryptAppPassword failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("EncryptAppPassword finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public EncryptAppPasswordDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new EncryptAppPassword(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
