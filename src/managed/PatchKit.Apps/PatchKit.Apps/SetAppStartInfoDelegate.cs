using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object SetAppStartInfoDelegate(App app, AppStartInfo? value, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
