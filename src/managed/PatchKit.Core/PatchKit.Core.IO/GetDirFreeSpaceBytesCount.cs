using System;
using System.Runtime.InteropServices;
using Autofac;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class GetDirFreeSpaceBytesCount
{
	public GetCurrentPlatformTypeDelegate GetCurrentPlatformType { get; }

	public DoesDirExistDelegate DoesDirExist { get; }

	public GetParentPathDelegate GetParentPath { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	[DllImport("kernel32", CharSet = CharSet.Auto, EntryPoint = "GetDiskFreeSpaceEx", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool kernel32_GetDiskFreeSpaceEx(string directoryName, out ulong freeBytes, out ulong totalBytes, out ulong totalFreeBytes);

	[DllImport("libgetdiskspace32", CharSet = CharSet.Auto, EntryPoint = "getAvailableDiskSpace", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool libgetdiskspace32_getAvailableDiskSpace(string t_path, out long freeBytes);

	[DllImport("libgetdiskspace64", CharSet = CharSet.Auto, EntryPoint = "getAvailableDiskSpace", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool libgetdiskspace64_getAvailableDiskSpace(string t_path, out long freeBytes);

	[DllImport("getdiskspace_osx_x64", CharSet = CharSet.Auto, EntryPoint = "getAvailableDiskSpace", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool getdiskspaceosx64_getAvailableDiskSpace(string t_path, out long freeBytes);

	private BytesCount Execute(Path path)
	{
		Path path2 = path;
		while (!DoesDirExist(path2, null))
		{
			Path? path3 = GetParentPath(path2);
			if (!path3.HasValue)
			{
				break;
			}
			path2 = path3.Value;
		}
		switch (GetCurrentPlatformType())
		{
		case PlatformType.Win64:
		case PlatformType.Win32:
		{
			kernel32_GetDiskFreeSpaceEx(path2.Value, out var freeBytes4, out var _, out var _);
			return new BytesCount((long)freeBytes4);
		}
		case PlatformType.Linux64:
		{
			libgetdiskspace64_getAvailableDiskSpace(path2.Value, out var freeBytes3);
			return new BytesCount(freeBytes3);
		}
		case PlatformType.Linux32:
		{
			libgetdiskspace32_getAvailableDiskSpace(path2.Value, out var freeBytes2);
			return new BytesCount(freeBytes2);
		}
		case PlatformType.Osx64:
		{
			getdiskspaceosx64_getAvailableDiskSpace(path2.Value, out var freeBytes);
			return new BytesCount(freeBytes);
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public GetDirFreeSpaceBytesCount(GetCurrentPlatformTypeDelegate getCurrentPlatformType, DoesDirExistDelegate doesDirExist, GetParentPathDelegate getParentPath, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getCurrentPlatformType);
		Assert.IsNotNull(doesDirExist);
		Assert.IsNotNull(getParentPath);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetCurrentPlatformType = getCurrentPlatformType;
		DoesDirExist = doesDirExist;
		GetParentPath = getParentPath;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private BytesCount DelegateExecute(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess)
	{
		Arg.IsValid(path, "path");
		LogInfo("Executing GetDirFreeSpaceBytesCount...");
		LogTrace("path = " + path);
		try
		{
			BeginLogRegion(enabled: true);
			BytesCount bytesCount = Execute(path);
			EndLogRegion(enabled: true);
			BytesCount bytesCount2 = bytesCount;
			LogTrace("result = " + bytesCount2.ToString());
			LogInfo("GetDirFreeSpaceBytesCount finished: success.");
			return bytesCount;
		}
		catch (UnauthorizedAccess e)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetDirFreeSpaceBytesCount finished: UnauthorizedAccess.");
			if (onUnauthorizedAccess == null)
			{
				throw;
			}
			object obj = onUnauthorizedAccess(e);
			if (obj == null)
			{
				return default;
			}
			return (BytesCount)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("GetDirFreeSpaceBytesCount has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("GetDirFreeSpaceBytesCount failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("GetDirFreeSpaceBytesCount finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetDirFreeSpaceBytesCountDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetDirFreeSpaceBytesCount(context.Resolve<GetCurrentPlatformTypeDelegate>(), context.Resolve<DoesDirExistDelegate>(), context.Resolve<GetParentPathDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
