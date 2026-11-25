using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object SetAppPasswordDelegate(App app, AppPassword appPassword, AppSecret appSecret, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
