using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate ProcessId StartAppDelegate(App app, Text? appProcessExtraArgsAsText, HandleExceptionAndReturnDelegate<AppRootDirDoesNotExist> onAppRootDirDoesNotExist, HandleExceptionAndReturnDelegate<UnauthorizedAccessToAppRootDir> onUnauthorizedAccessToAppRootDir, HandleExceptionAndReturnDelegate<AppIsNotInstalled> onAppIsNotInstalled, HandleExceptionAndReturnDelegate<AppNeedsRepair> onAppNeedsRepair);
