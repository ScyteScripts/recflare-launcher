namespace PatchKit.Apps;

public enum AppInstallTaskError
{
	Fatal,
	Cancelled,
	OutOfAppDirFreeSpace,
	UnauthorizedAccessToAppDir,
	RequiredAppLicenseKey,
	InvalidAppLicenseKey,
	BlockedAppLicenseKey,
	InvalidAppAuthKey,
	ExpiredAppAuthKey,
	InvalidAppPassword
}
