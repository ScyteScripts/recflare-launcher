using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppInfo : IValidatedStruct
{
	public AppHashedSecret? HashedSecret { get; }

	public AppLicenseKey? LicenseKey { get; }

	public AppEncryptedLicenseSecret? EncryptedLicenseSecret { get; }

	public AppEncryptedPassword? EncryptedPassword { get; }

	public AppVersionId? InstalledVersionId { get; }

	public AppVersionId? LastInstalledVersionId { get; }

	public AppStartInfo? StartInfo { get; }

	public AppLastFilesOperationTaskState? LastFilesOperationTaskState { get; }

	public AppLastUnfinishedWriteFileDataOperationInfo? LastUnfinishedWriteFileDataOperationInfo { get; }

	public bool IsValid { get; }

	public static string Validate(AppHashedSecret? hashedSecret, AppLicenseKey? licenseKey, AppEncryptedLicenseSecret? encryptedLicenseSecret, AppEncryptedPassword? encryptedPassword, AppVersionId? installedVersionId, AppVersionId? lastInstalledVersionId, AppStartInfo? startInfo, AppLastFilesOperationTaskState? lastFilesOperationTaskState, AppLastUnfinishedWriteFileDataOperationInfo? lastUnfinishedWriteFileDataOperationInfo)
	{
		return null;
	}

	public AppInfo(AppHashedSecret? hashedSecret, AppLicenseKey? licenseKey, AppEncryptedLicenseSecret? encryptedLicenseSecret, AppEncryptedPassword? encryptedPassword, AppVersionId? installedVersionId, AppVersionId? lastInstalledVersionId, AppStartInfo? startInfo, AppLastFilesOperationTaskState? lastFilesOperationTaskState, AppLastUnfinishedWriteFileDataOperationInfo? lastUnfinishedWriteFileDataOperationInfo)
	{
		this = default;
		Arg.IsValid(hashedSecret, "hashedSecret");
		Arg.IsValid(licenseKey, "licenseKey");
		Arg.IsValid(encryptedLicenseSecret, "encryptedLicenseSecret");
		Arg.IsValid(encryptedPassword, "encryptedPassword");
		Arg.IsValid(installedVersionId, "installedVersionId");
		Arg.IsValid(lastInstalledVersionId, "lastInstalledVersionId");
		Arg.IsValid(startInfo, "startInfo");
		Arg.IsValid(lastFilesOperationTaskState, "lastFilesOperationTaskState");
		Arg.IsValid(lastUnfinishedWriteFileDataOperationInfo, "lastUnfinishedWriteFileDataOperationInfo");
		string text = Validate(hashedSecret, licenseKey, encryptedLicenseSecret, encryptedPassword, installedVersionId, lastInstalledVersionId, startInfo, lastFilesOperationTaskState, lastUnfinishedWriteFileDataOperationInfo);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		HashedSecret = hashedSecret;
		LicenseKey = licenseKey;
		EncryptedLicenseSecret = encryptedLicenseSecret;
		EncryptedPassword = encryptedPassword;
		InstalledVersionId = installedVersionId;
		LastInstalledVersionId = lastInstalledVersionId;
		StartInfo = startInfo;
		LastFilesOperationTaskState = lastFilesOperationTaskState;
		LastUnfinishedWriteFileDataOperationInfo = lastUnfinishedWriteFileDataOperationInfo;
		IsValid = true;
	}

	public AppInfo ModifyHashedSecret(AppHashedSecret? newHashedSecret)
	{
		return new AppInfo(newHashedSecret, LicenseKey, EncryptedLicenseSecret, EncryptedPassword, InstalledVersionId, LastInstalledVersionId, StartInfo, LastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyLicenseKey(AppLicenseKey? newLicenseKey)
	{
		return new AppInfo(HashedSecret, newLicenseKey, EncryptedLicenseSecret, EncryptedPassword, InstalledVersionId, LastInstalledVersionId, StartInfo, LastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyEncryptedLicenseSecret(AppEncryptedLicenseSecret? newEncryptedLicenseSecret)
	{
		return new AppInfo(HashedSecret, LicenseKey, newEncryptedLicenseSecret, EncryptedPassword, InstalledVersionId, LastInstalledVersionId, StartInfo, LastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyEncryptedPassword(AppEncryptedPassword? newEncryptedPassword)
	{
		return new AppInfo(HashedSecret, LicenseKey, EncryptedLicenseSecret, newEncryptedPassword, InstalledVersionId, LastInstalledVersionId, StartInfo, LastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyInstalledVersionId(AppVersionId? newInstalledVersionId)
	{
		return new AppInfo(HashedSecret, LicenseKey, EncryptedLicenseSecret, EncryptedPassword, newInstalledVersionId, LastInstalledVersionId, StartInfo, LastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyLastInstalledVersionId(AppVersionId? newLastInstalledVersionId)
	{
		return new AppInfo(HashedSecret, LicenseKey, EncryptedLicenseSecret, EncryptedPassword, InstalledVersionId, newLastInstalledVersionId, StartInfo, LastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyStartInfo(AppStartInfo? newStartInfo)
	{
		return new AppInfo(HashedSecret, LicenseKey, EncryptedLicenseSecret, EncryptedPassword, InstalledVersionId, LastInstalledVersionId, newStartInfo, LastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyLastFilesOperationTaskState(AppLastFilesOperationTaskState? newLastFilesOperationTaskState)
	{
		return new AppInfo(HashedSecret, LicenseKey, EncryptedLicenseSecret, EncryptedPassword, InstalledVersionId, LastInstalledVersionId, StartInfo, newLastFilesOperationTaskState, LastUnfinishedWriteFileDataOperationInfo);
	}

	public AppInfo ModifyLastUnfinishedWriteFileDataOperationInfo(AppLastUnfinishedWriteFileDataOperationInfo? newLastUnfinishedWriteFileDataOperationInfo)
	{
		return new AppInfo(HashedSecret, LicenseKey, EncryptedLicenseSecret, EncryptedPassword, InstalledVersionId, LastInstalledVersionId, StartInfo, LastFilesOperationTaskState, newLastUnfinishedWriteFileDataOperationInfo);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"HashedSecret\" = {HashedSecret}, \"LicenseKey\" = {LicenseKey}, \"EncryptedLicenseSecret\" = {EncryptedLicenseSecret}, \"EncryptedPassword\" = {EncryptedPassword}, \"InstalledVersionId\" = {InstalledVersionId}, \"LastInstalledVersionId\" = {LastInstalledVersionId}, \"StartInfo\" = {StartInfo}, \"LastFilesOperationTaskState\" = {LastFilesOperationTaskState}, \"LastUnfinishedWriteFileDataOperationInfo\" = {LastUnfinishedWriteFileDataOperationInfo}";
		return text + "}";
	}
}
