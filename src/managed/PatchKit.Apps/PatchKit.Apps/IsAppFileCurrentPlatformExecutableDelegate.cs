using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate bool IsAppFileCurrentPlatformExecutableDelegate(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingAppFile> onNotExistingAppFile);
