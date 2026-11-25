using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public delegate object UninstallAppDelegate(App app, HandleExceptionDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir);
