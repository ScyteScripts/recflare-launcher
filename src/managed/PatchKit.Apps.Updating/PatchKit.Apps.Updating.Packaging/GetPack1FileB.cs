using System;
using Autofac;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public class GetPack1FileB
{
	public GetPack1FileStreamDelegate GetPack1FileStream { get; }

	public GetPack1FileADelegate GetPack1FileA { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private IPackageFileDisposable Execute(AppRemotePackageFileInfo info, IPack1FileSourceStream sourceStream, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password)
	{
		IPackageFileStreamDisposable packageFileStreamDisposable = GetPack1FileStream(sourceStream, encryption, compressionMethod, password);
		Assert.IsNotNullFromDelegate(packageFileStreamDisposable);
		try
		{
			IPackageFileDisposable packageFileDisposable = GetPack1FileA(info, packageFileStreamDisposable);
			Assert.IsNotNullFromDelegate(packageFileDisposable);
			packageFileDisposable.AttachDisposable(packageFileStreamDisposable);
			return packageFileDisposable;
		}
		catch (Exception)
		{
			packageFileStreamDisposable.Dispose();
			throw;
		}
	}

	public GetPack1FileB(GetPack1FileStreamDelegate getPack1FileStream, GetPack1FileADelegate getPack1FileA, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getPack1FileStream);
		Assert.IsNotNull(getPack1FileA);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetPack1FileStream = getPack1FileStream;
		GetPack1FileA = getPack1FileA;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private IPackageFileDisposable DelegateExecute(AppRemotePackageFileInfo info, IPack1FileSourceStream sourceStream, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password)
	{
		Arg.IsValid(info, "info");
		Arg.IsNotNull(sourceStream, "sourceStream");
		Arg.IsValid(encryption, "encryption");
		Arg.IsValid(password, "password");
		LogInfo("Executing GetPack1FileB...");
		AppRemotePackageFileInfo appRemotePackageFileInfo = info;
		LogTrace("info = " + appRemotePackageFileInfo.ToString());
		LogTrace("sourceStream = " + sourceStream);
		AppRemotePack1Encryption appRemotePack1Encryption = encryption;
		LogTrace("encryption = " + appRemotePack1Encryption.ToString());
		LogTrace("compressionMethod = " + compressionMethod);
		LogTrace("password = " + password);
		try
		{
			BeginLogRegion(enabled: true);
			IPackageFileDisposable packageFileDisposable = Execute(info, sourceStream, encryption, compressionMethod, password);
			EndLogRegion(enabled: true);
			LogTrace("result = " + packageFileDisposable);
			LogInfo("GetPack1FileB finished: success.");
			return packageFileDisposable;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetPack1FileB has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileB failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetPack1FileB finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetPack1FileBDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetPack1FileB(context.Resolve<GetPack1FileStreamDelegate>(), context.Resolve<GetPack1FileADelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
