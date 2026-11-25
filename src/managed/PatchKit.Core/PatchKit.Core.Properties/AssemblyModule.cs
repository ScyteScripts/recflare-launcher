using Autofac;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Core.Properties;

public class AssemblyModule : Module
{
	private readonly BeginLogRegionDelegate _beginLogRegion;

	private readonly EndLogRegionDelegate _endLogRegion;

	private readonly GetCurrentPlatformTypeDelegate _getCurrentPlatformType;

	private readonly LogDelegate _log;

	public AssemblyModule(PlatformType platformType, LogDelegate log = null, BeginLogRegionDelegate beginLogRegion = null, EndLogRegionDelegate endLogRegion = null)
	{
		_getCurrentPlatformType = () => platformType;
		_log = log ?? ((LogDelegate)((LogMessage _) =>
		{
		}));
		_beginLogRegion = beginLogRegion ?? ((BeginLogRegionDelegate)((bool enabled) =>
		{
		}));
		_endLogRegion = endLogRegion ?? ((EndLogRegionDelegate)((bool enabled) =>
		{
		}));
	}

	protected override void Load(ContainerBuilder builder)
	{
		base.Load(builder);
		builder.Register((IComponentContext context) => _getCurrentPlatformType);
		builder.Register((IComponentContext context) => _log);
		builder.Register((IComponentContext context) => _beginLogRegion);
		builder.Register((IComponentContext context) => _endLogRegion);
		AutoLoad(builder);
	}

	private void AutoLoad(ContainerBuilder builder)
	{
		AddFileExecutableRightsIfRequired.RegisterInContainer(builder);
		CombinePath.RegisterInContainer(builder);
		CopyFile.RegisterInContainer(builder);
		CreateDir.RegisterInContainer(builder);
		CreateFile.RegisterInContainer(builder);
		CreateParentDir.RegisterInContainer(builder);
		DeleteDir.RegisterInContainer(builder);
		DeleteDirIfExists.RegisterInContainer(builder);
		DeleteDirRecursively.RegisterInContainer(builder);
		DeleteDirRecursivelyIfExists.RegisterInContainer(builder);
		DeleteFile.RegisterInContainer(builder);
		DeleteFileIfExists.RegisterInContainer(builder);
		DoesBytesRangeInclude.RegisterInContainer(builder);
		DoesDirExist.RegisterInContainer(builder);
		DoesFileExist.RegisterInContainer(builder);
		DoesFileMagicBytesMatch.RegisterInContainer(builder);
		GetDirFreeSpaceBytesCount.RegisterInContainer(builder);
		GetFileLock.RegisterInContainer(builder);
		GetFileSize.RegisterInContainer(builder);
		GetJointBytesRange.RegisterInContainer(builder);
		GetParentPath.RegisterInContainer(builder);
		GetTempDir.RegisterInContainer(builder);
		GetBufferxxHash.RegisterInContainer(builder);
		GetFilexxHash.RegisterInContainer(builder);
		IsDirEmpty.RegisterInContainer(builder);
		IsFileCurrentPlatformExecutable.RegisterInContainer(builder);
		IsFileELFExecutable.RegisterInContainer(builder);
		IsFileOSXExecutable.RegisterInContainer(builder);
		IsFileUnixExecutable.RegisterInContainer(builder);
		IsFileWindowsExecutable.RegisterInContainer(builder);
		MoveDir.RegisterInContainer(builder);
		MoveDirOrFileToSafePlace.RegisterInContainer(builder);
		MoveFile.RegisterInContainer(builder);
		MoveFileOverwrite.RegisterInContainer(builder);
		ReadTextFromFile.RegisterInContainer(builder);
		StartProcess.RegisterInContainer(builder);
		WriteTextToFile.RegisterInContainer(builder);
	}
}
