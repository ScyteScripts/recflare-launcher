using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public delegate object SetAppRootDirPermissionsDelegate(App app, HandleExceptionDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir);
