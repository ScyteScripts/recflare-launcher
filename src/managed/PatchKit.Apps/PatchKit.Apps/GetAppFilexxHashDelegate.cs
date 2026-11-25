using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps;

public delegate Hash GetAppFilexxHashDelegate(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingAppFile> onNotExistingAppFile);
