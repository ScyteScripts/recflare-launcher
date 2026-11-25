using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object DeleteAppFileDelegate(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingAppFile> onNotExistingAppFile);
