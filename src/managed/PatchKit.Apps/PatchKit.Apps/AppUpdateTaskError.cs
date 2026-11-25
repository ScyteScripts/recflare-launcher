namespace PatchKit.Apps;

public enum AppUpdateTaskError
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
