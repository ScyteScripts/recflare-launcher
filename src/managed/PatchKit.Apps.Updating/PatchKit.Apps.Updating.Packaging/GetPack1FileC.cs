using System;
using System.Threading;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPack1FileC
{
	public GetPack1FileSourceStreamDelegate GetPack1FileSourceStream { get; }

	public GetPack1FileBDelegate GetPack1FileB { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageFileDisposable Execute(AppRemotePackageFileInfo info, IPackageStream packageStream, AppRemotePack1FileUnpackInfo unpackInfo, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password, CancellationToken cancellationToken)
	{
		IPack1FileSourceStreamDisposable pack1FileSourceStreamDisposable = GetPack1FileSourceStream(packageStream, unpackInfo.Offset, unpackInfo.Size, cancellationToken, null, null, null);
		Assert.IsNotNullFromDelegate(pack1FileSourceStreamDisposable);
		try
		{
			IPackageFileDisposable packageFileDisposable = GetPack1FileB(info, pack1FileSourceStreamDisposable, encryption, compressionMethod, password);
			Assert.IsNotNullFromDelegate(packageFileDisposable);
			packageFileDisposable.AttachDisposable(pack1FileSourceStreamDisposable);
			return packageFileDisposable;
		}
		catch (Exception)
		{
			pack1FileSourceStreamDisposable.Dispose();
			throw;
		}
	}

	public GetPack1FileC(GetPack1FileSourceStreamDelegate getPack1FileSourceStream, GetPack1FileBDelegate getPack1FileB, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getPack1FileSourceStream);
		Assert.IsNotNull(getPack1FileB);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetPack1FileSourceStream = getPack1FileSourceStream;
		GetPack1FileB = getPack1FileB;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageFileDisposable DelegateExecute(AppRemotePackageFileInfo info, IPackageStream packageStream, AppRemotePack1FileUnpackInfo unpackInfo, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<PackageFileSourceStreamOutOfRange> onPackageFileSourceStreamOutOfRange, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey)
	{
		Arg.IsValid(info, "info");
		Arg.IsNotNull(packageStream, "packageStream");
		Arg.IsValid(unpackInfo, "unpackInfo");
		Arg.IsValid(encryption, "encryption");
		Arg.IsValid(password, "password");
		LogInfo("Executing GetPack1FileC...");
		AppRemotePackageFileInfo appRemotePackageFileInfo = info;
		LogTrace("info = " + appRemotePackageFileInfo.ToString());
		LogTrace("packageStream = " + packageStream);
		AppRemotePack1FileUnpackInfo appRemotePack1FileUnpackInfo = unpackInfo;
		LogTrace("unpackInfo = " + appRemotePack1FileUnpackInfo.ToString());
		AppRemotePack1Encryption appRemotePack1Encryption = encryption;
		LogTrace("encryption = " + appRemotePack1Encryption.ToString());
		LogTrace("compressionMethod = " + compressionMethod);
		LogTrace("password = " + password);
		LogTrace("cancellationToken = " + cancellationToken);
		try
		{
			BeginLogRegion(enabled: true);
			IPackageFileDisposable packageFileDisposable = Execute(info, packageStream, unpackInfo, encryption, compressionMethod, password, cancellationToken);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageFileDisposable);
			LogInfo("GetPack1FileC finished: success.");
			return packageFileDisposable;
		}
		catch (PackageFileSourceStreamOutOfRange e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileC finished: PackageFileSourceStreamOutOfRange.");
			if (onPackageFileSourceStreamOutOfRange == null)
			{
				throw;
			}
			object obj = onPackageFileSourceStreamOutOfRange(e);
			if (obj == null)
			{
				return null;
			}
			return (IPackageFileDisposable)obj;
		}
		catch (InvalidAppAuthKey e2)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileC finished: InvalidAppAuthKey.");
			if (onInvalidAppAuthKey == null)
			{
				throw;
			}
			object obj2 = onInvalidAppAuthKey(e2);
			if (obj2 == null)
			{
				return null;
			}
			return (IPackageFileDisposable)obj2;
		}
		catch (ExpiredAppAuthKey e3)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileC finished: ExpiredAppAuthKey.");
			if (onExpiredAppAuthKey == null)
			{
				throw;
			}
			object obj3 = onExpiredAppAuthKey(e3);
			if (obj3 == null)
			{
				return null;
			}
			return (IPackageFileDisposable)obj3;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileC has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileC failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileC finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPack1FileCDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPack1FileC(context.Resolve<GetPack1FileSourceStreamDelegate>(), context.Resolve<GetPack1FileBDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
