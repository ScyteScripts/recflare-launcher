namespace PatchKit.Apps;

public enum AppRepairTaskError
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
