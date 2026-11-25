using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public delegate bool DoesAppRootDirExistDelegate(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir);
