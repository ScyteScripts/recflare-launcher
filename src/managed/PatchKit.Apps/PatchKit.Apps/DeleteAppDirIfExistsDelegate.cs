using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object DeleteAppDirIfExistsDelegate(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
