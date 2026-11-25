using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public delegate object SetAppLastInstalledVersionIdDelegate(App app, AppVersionId? value, HandleExceptionDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir);
