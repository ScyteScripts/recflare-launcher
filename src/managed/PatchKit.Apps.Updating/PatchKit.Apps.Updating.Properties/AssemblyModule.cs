using Autofac;
using PatchKit.Apps.Properties;
using PatchKit.Apps.Remote.Properties;
using PatchKit.Apps.Updating.Installation;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Apps.Updating.Strategy;
using PatchKit.Core.Properties;
using PatchKit.Network.Properties;

namespace PatchKit.Apps.Updating.Properties;

public class AssemblyModule : Module
{
	private readonly PatchKit.Apps.Properties.AssemblyModule _appsModule;

	private readonly PatchKit.Core.Properties.AssemblyModule _coreModule;

	private readonly PatchKit.Network.Properties.AssemblyModule _networkModule;

	private readonly PatchKit.Apps.Remote.Properties.AssemblyModule _appsRemoteModule;

	public AssemblyModule(PatchKit.Core.Properties.AssemblyModule coreModule, PatchKit.Network.Properties.AssemblyModule networkModule, PatchKit.Apps.Properties.AssemblyModule appsModule, PatchKit.Apps.Remote.Properties.AssemblyModule appsRemoteModule)
	{
		_coreModule = coreModule;
		_networkModule = networkModule;
		_appsModule = appsModule;
		_appsRemoteModule = appsRemoteModule;
	}

	protected override void Load(ContainerBuilder builder)
	{
		base.Load(builder);
		builder.RegisterModule(_coreModule);
		builder.RegisterModule(_networkModule);
		builder.RegisterModule(_appsModule);
		builder.RegisterModule(_appsRemoteModule);
		AutoLoad(builder);
	}

	private void AutoLoad(ContainerBuilder builder)
	{
		CreateAppDirs.RegisterInContainer(builder);
		DeleteAllAppEntries.RegisterInContainer(builder);
		DeleteAppExtraEntries.RegisterInContainer(builder);
		DeleteRedundantAppEntries.RegisterInContainer(builder);
		DoesDirHaveFreeSpaceForUpdatingApp.RegisterInContainer(builder);
		InstallPackage.RegisterInContainer(builder);
		InstallPackageDataFile.RegisterInContainer(builder);
		InstallPackageFile.RegisterInContainer(builder);
		ProcessInstallPackagePatchFileTasksQueue.RegisterInContainer(builder);
		ScheduleInstallPackagePatchFileTask.RegisterInContainer(builder);
		WritePackageFileStreamToFile.RegisterInContainer(builder);
		WritePackageFileStreamToFileStream.RegisterInContainer(builder);
		GetFileUpdateStrategiesPackagesInfo.RegisterInContainer(builder);
		GetPack1.RegisterInContainer(builder);
		GetPack1FileA.RegisterInContainer(builder);
		GetPack1FileB.RegisterInContainer(builder);
		GetPack1FileC.RegisterInContainer(builder);
		GetPack1FileSourceStream.RegisterInContainer(builder);
		GetPack1FileStream.RegisterInContainer(builder);
		GetPackage.RegisterInContainer(builder);
		GetPackageMultisourceStream.RegisterInContainer(builder);
		GetPackageSourcePartStream.RegisterInContainer(builder);
		GetPackageSourcePartStreamRequest.RegisterInContainer(builder);
		GetPackageSourceStream.RegisterInContainer(builder);
		GetPackageStream.RegisterInContainer(builder);
		GetPackageWithChunksStream.RegisterInContainer(builder);
		GetPackageWithoutChunksStream.RegisterInContainer(builder);
		GetZip.RegisterInContainer(builder);
		GetZipFile.RegisterInContainer(builder);
		MovePackageStreamPos.RegisterInContainer(builder);
		GetDirsToCreate.RegisterInContainer(builder);
		GetEntriesToDelete.RegisterInContainer(builder);
		GetFilesUpdateStrategy.RegisterInContainer(builder);
		GetFileUpdateStrategy.RegisterInContainer(builder);
		GetUpdateFileState.RegisterInContainer(builder);
		GetUpdateStrategy.RegisterInContainer(builder);
		IsFileUpdatePatchesStrategyPossible.RegisterInContainer(builder);
		IsUpdateFileConsistent.RegisterInContainer(builder);
		UninstallApp.RegisterInContainer(builder);
		UpdateApp.RegisterInContainer(builder);
		UpdateAppFiles.RegisterInContainer(builder);
		UpdateAppStartInfo.RegisterInContainer(builder);
		ValidateAndSaveAppLicenseKey.RegisterInContainer(builder);
		ValidateAppLicenseKeyA.RegisterInContainer(builder);
		ValidateAppLicenseKeyB.RegisterInContainer(builder);
	}
}
