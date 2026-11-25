namespace PatchKit.Apps;

public enum AppMoveTaskError
{
	Fatal,
	Cancelled,
	OutOfAppDirFreeSpace,
	UnauthorizedAccessToAppDir,
	UnauthorizedAccessToAppNewDir
}
