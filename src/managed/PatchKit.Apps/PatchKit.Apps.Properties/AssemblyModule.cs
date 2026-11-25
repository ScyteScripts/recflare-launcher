using Autofac;
using PatchKit.Apps.Serialization;
using PatchKit.Core.Properties;

namespace PatchKit.Apps.Properties;

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
		AddAppFileExecutableRightsIfRequired.RegisterInContainer(builder);
		AddExecutableRightsToAppFiles.RegisterInContainer(builder);
		CleanUnregisteredAppEmptyParentDirs.RegisterInContainer(builder);
		CreateAppDir.RegisterInContainer(builder);
		CreateAppFile.RegisterInContainer(builder);
		CreateAppRootDir.RegisterInContainer(builder);
		DecryptAppEncryptedPassword.RegisterInContainer(builder);
		DecryptAppLicenseSecret.RegisterInContainer(builder);
		DeleteAppDir.RegisterInContainer(builder);
		DeleteAppDirIfExists.RegisterInContainer(builder);
		DeleteAppFile.RegisterInContainer(builder);
		DeleteAppFileIfExists.RegisterInContainer(builder);
		DeleteAppFileOrDir.RegisterInContainer(builder);
		DeleteAppMetaDataEntry.RegisterInContainer(builder);
		DeleteAppMetaDataFile.RegisterInContainer(builder);
		DoesAppDirExist.RegisterInContainer(builder);
		DoesAppFileExist.RegisterInContainer(builder);
		DoesAppNeedRepair.RegisterInContainer(builder);
		DoesAppRootDirExist.RegisterInContainer(builder);
		EncryptAppLicenseSecret.RegisterInContainer(builder);
		EncryptAppPassword.RegisterInContainer(builder);
		FindAppStartExecutablePath.RegisterInContainer(builder);
		GenerateAppStartExecutableArgs.RegisterInContainer(builder);
		GenerateAppStartInfo.RegisterInContainer(builder);
		GetAppDirPaths.RegisterInContainer(builder);
		GetAppEncryptedLicenseSecret.RegisterInContainer(builder);
		GetAppEncryptedPassword.RegisterInContainer(builder);
		GetAppEntriesInfo.RegisterInContainer(builder);
		GetAppEntryFullPath.RegisterInContainer(builder);
		GetAppEntryInfo.RegisterInContainer(builder);
		GetAppEntryPaths.RegisterInContainer(builder);
		GetAppExtraEntriesInfoFilePath.RegisterInContainer(builder);
		GetAppFilePaths.RegisterInContainer(builder);
		GetAppFileSize.RegisterInContainer(builder);
		GetAppFilexxHash.RegisterInContainer(builder);
		GetAppInfo.RegisterInContainer(builder);
		GetAppInstalledVersionId.RegisterInContainer(builder);
		GetAppLastFilesOperationTaskState.RegisterInContainer(builder);
		GetAppLastInstalledVersionId.RegisterInContainer(builder);
		GetAppLastUnfinishedWriteFileDataOperationInfo.RegisterInContainer(builder);
		GetAppLicenseKey.RegisterInContainer(builder);
		GetAppLicenseSecret.RegisterInContainer(builder);
		GetAppMetaDataDeprecatedFilePaths.RegisterInContainer(builder);
		GetAppMetaDataEntry.RegisterInContainer(builder);
		GetAppMetaDataFilePath.RegisterInContainer(builder);
		GetAppPassword.RegisterInContainer(builder);
		GetAppStartInfo.RegisterInContainer(builder);
		GetDefaultAppMetaData.RegisterInContainer(builder);
		GetParentAppPath.RegisterInContainer(builder);
		IsAppFileCurrentPlatformExecutable.RegisterInContainer(builder);
		IsAppInstalled.RegisterInContainer(builder);
		MoveApp.RegisterInContainer(builder);
		PatchAppFile.RegisterInContainer(builder);
		ReadAppExtraEntriesInfo.RegisterInContainer(builder);
		ReadAppMetaData.RegisterInContainer(builder);
		ReadAppMetaDataFromFile.RegisterInContainer(builder);
		DeserializeExtraEntriesInfo.RegisterInContainer(builder);
		DeserializeMetaData.RegisterInContainer(builder);
		DeserializeMetaDataFormatVersion.RegisterInContainer(builder);
		DeserializeMetaDataFrom1_x_x.RegisterInContainer(builder);
		DeserializeMetaDataFrom2_x_x.RegisterInContainer(builder);
		DeserializeMetaDataFromUnversionedFormat.RegisterInContainer(builder);
		ParseMetaDataFormatVersion.RegisterInContainer(builder);
		SerializeMetaDataTo1_1_0.RegisterInContainer(builder);
		SerializeMetaDataTo2_0_0.RegisterInContainer(builder);
		SetAppInfo.RegisterInContainer(builder);
		SetAppInstalledVersionId.RegisterInContainer(builder);
		SetAppLastFilesOperationTaskState.RegisterInContainer(builder);
		SetAppLastInstalledVersionId.RegisterInContainer(builder);
		SetAppLastUnfinishedWriteFileDataOperationInfo.RegisterInContainer(builder);
		SetAppLicenseKeyAndSecret.RegisterInContainer(builder);
		SetAppMetaDataEntry.RegisterInContainer(builder);
		SetAppPassword.RegisterInContainer(builder);
		SetAppRootDirPermissions.RegisterInContainer(builder);
		SetAppStartInfo.RegisterInContainer(builder);
		StartApp.RegisterInContainer(builder);
		WriteAppMetaDataCache.RegisterInContainer(builder);
	}
}
