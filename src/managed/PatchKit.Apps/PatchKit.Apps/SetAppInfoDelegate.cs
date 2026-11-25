using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object SetAppInfoDelegate(App app, AppInfo value, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
