using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public delegate object SetAppInstalledVersionIdDelegate(App app, AppVersionId? value, HandleExceptionDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir);
