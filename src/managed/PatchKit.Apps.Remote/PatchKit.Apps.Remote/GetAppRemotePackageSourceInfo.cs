using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public class GetAppRemotePackageSourceInfo
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private AppRemotePackageSourceInfo Execute(HttpAddress address, BytesCount? partSize, BytesCount offset, BytesCount size, Text? authHeaderValue)
	{
		if (!partSize.HasValue)
		{
			return new AppRemotePackageSourceInfo(new AppRemotePackageSourcePartInfo[1]
			{
				new AppRemotePackageSourcePartInfo(address, offset, size, authHeaderValue)
			}.AsImmutable());
		}
		List<AppRemotePackageSourcePartInfo> list = new List<AppRemotePackageSourcePartInfo>();
		long num = size;
		long num2 = (long)offset / (long)partSize.Value;
		long num3 = (long)offset - num2 * (long)partSize.Value;
		long num4 = num2;
		while (num > 0)
		{
			string text = address.ToString();
			if (num4 > 0)
			{
				text += $".{num4}";
			}
			long num5 = ((num4 == num2) ? num3 : 0);
			long val = (long)partSize.Value - num5;
			long num6 = Math.Min(val, num);
			list.Add(new AppRemotePackageSourcePartInfo(new HttpAddress(new Uri(text)), new BytesCount(num5), new BytesCount(num6), authHeaderValue));
			num -= num6;
			num4++;
		}
		bool v = num == 0;
		Assert.IsTrue(v, "doesLeftSizeEqual0");
		long num7 = list.Sum((AppRemotePackageSourcePartInfo x) => x.Size);
		bool v2 = num7 == (long)size;
		Assert.IsTrue(v2, "isPartsSizeSumCorrect");
		return new AppRemotePackageSourceInfo(list.AsImmutable());
	}

	public GetAppRemotePackageSourceInfo(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private AppRemotePackageSourceInfo DelegateExecute(HttpAddress address, BytesCount? partSize, BytesCount offset, BytesCount size, Text? authHeaderValue)
	{
		Arg.IsValid(address, "address");
		Arg.IsValid(partSize, "partSize");
		Arg.IsValid(offset, "offset");
		Arg.IsValid(size, "size");
		Arg.IsValid(authHeaderValue, "authHeaderValue");
		LogInfo("Executing GetAppRemotePackageSourceInfo...");
		HttpAddress httpAddress = address;
		LogTrace("address = " + httpAddress.ToString());
		BytesCount? bytesCount = partSize;
		LogTrace("partSize = " + bytesCount.ToString());
		BytesCount bytesCount2 = offset;
		LogTrace("offset = " + bytesCount2.ToString());
		bytesCount2 = size;
		LogTrace("size = " + bytesCount2.ToString());
		Text? text = authHeaderValue;
		LogTrace("authHeaderValue = " + (text.HasValue ? ((string)text.GetValueOrDefault()) : null));
		try
		{
			BeginLogRegion(enabled: false);
			AppRemotePackageSourceInfo appRemotePackageSourceInfo = Execute(address, partSize, offset, size, authHeaderValue);
			EndLogRegion(enabled: false);
			AppRemotePackageSourceInfo appRemotePackageSourceInfo2 = appRemotePackageSourceInfo;
			LogTrace("result = " + appRemotePackageSourceInfo2.ToString());
			LogInfo("GetAppRemotePackageSourceInfo finished: success.");
			return appRemotePackageSourceInfo;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetAppRemotePackageSourceInfo has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePackageSourceInfo failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetAppRemotePackageSourceInfo finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetAppRemotePackageSourceInfoDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetAppRemotePackageSourceInfo(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
