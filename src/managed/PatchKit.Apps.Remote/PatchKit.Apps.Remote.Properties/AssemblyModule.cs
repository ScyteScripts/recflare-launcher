using Autofac;
using PatchKit.Core.Properties;

namespace PatchKit.Apps.Remote.Properties;

public class AssemblyModule : Module
{
	private readonly PatchKit.Core.Properties.AssemblyModule _coreModule;

	public AssemblyModule(PatchKit.Core.Properties.AssemblyModule coreModule)
	{
		_coreModule = coreModule;
	}

	protected override void Load(ContainerBuilder builder)
	{
		base.Load(builder);
		builder.RegisterModule(_coreModule);
		AutoLoad(builder);
	}

	private void AutoLoad(ContainerBuilder builder)
	{
		DoesAppRemoteDiffContainsDataFile.RegisterInContainer(builder);
		DoesAppRemoteDiffContainsPatchFile.RegisterInContainer(builder);
		GetAppRemoteContentDataFileInfo.RegisterInContainer(builder);
		GetAppRemoteContentInfo.RegisterInContainer(builder);
		GetAppRemoteDiffDataFileInfo.RegisterInContainer(builder);
		GetAppRemoteDiffInfo.RegisterInContainer(builder);
		GetAppRemoteDiffPatchFileInfo.RegisterInContainer(builder);
		GetAppRemoteInfo.RegisterInContainer(builder);
		GetAppRemoteLatestVersionId.RegisterInContainer(builder);
		GetAppRemoteLowestVersionWithDiffId.RegisterInContainer(builder);
		GetAppRemotePack1UnpackInfoA.RegisterInContainer(builder);
		GetAppRemotePack1UnpackInfoB.RegisterInContainer(builder);
		GetAppRemotePackageMetaText.RegisterInContainer(builder);
		GetAppRemotePackageSourceInfo.RegisterInContainer(builder);
		GetAppRemotePackageSourcesInfo.RegisterInContainer(builder);
		GetAppRemotePackageUnpackPassword.RegisterInContainer(builder);
		GetAppRemoteStartInfo.RegisterInContainer(builder);
		GetAppRemoteVersionDirPaths.RegisterInContainer(builder);
		GetAppRemoteVersionDirsInfo.RegisterInContainer(builder);
		GetAppRemoteVersionEntriesInfo.RegisterInContainer(builder);
		GetAppRemoteVersionEntryPaths.RegisterInContainer(builder);
		GetAppRemoteVersionFileInfo.RegisterInContainer(builder);
		GetAppRemoteVersionFilePaths.RegisterInContainer(builder);
		GetAppRemoteVersionFilesInfo.RegisterInContainer(builder);
		GetAppRemoteVersionMetaInfo.RegisterInContainer(builder);
		GetCountryCode.RegisterInContainer(builder);
		GetFilteredAppRemoteChunkNewIndex.RegisterInContainer(builder);
		GetFilteredAppRemotePack1FilesUnpackInfo.RegisterInContainer(builder);
		GetFilteredAppRemotePack1FileUnpackInfo.RegisterInContainer(builder);
		GetFilteredAppRemotePackageBytesRanges.RegisterInContainer(builder);
		GetFilteredAppRemotePackageChunksRanges.RegisterInContainer(builder);
		GetFilteredNewAppRemoteContentInfo.RegisterInContainer(builder);
		GetFilteredNewAppRemoteDiffInfo.RegisterInContainer(builder);
		GetFilteredNewAppRemotePack1FilesUnpackInfo.RegisterInContainer(builder);
		GetFilteredNewAppRemotePack1FileUnpackInfo.RegisterInContainer(builder);
		GetFilteredNewAppRemotePack1Info.RegisterInContainer(builder);
		GetFilteredNewAppRemotePackageChunksInfo.RegisterInContainer(builder);
		GetFilteredNewAppRemotePackageInfoA.RegisterInContainer(builder);
		GetFilteredNewAppRemotePackageInfoB.RegisterInContainer(builder);
		GetFilteredNewAppRemotePackageSourceInfo.RegisterInContainer(builder);
		GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRange.RegisterInContainer(builder);
		GetFilteredNewAppRemotePackageSourcesInfo.RegisterInContainer(builder);
		GetFilteredNewAppRemoteZipInfo.RegisterInContainer(builder);
		GetMergedFilteredAppRemotePackageChunksRange.RegisterInContainer(builder);
		IsAppRemoteAuthTokenRequired.RegisterInContainer(builder);
		IsAppRemoteLicenseSecretRequired.RegisterInContainer(builder);
		IsAppRemotePasswordRequired.RegisterInContainer(builder);
		ParseAppRemoteHash.RegisterInContainer(builder);
	}
}
