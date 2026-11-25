using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate AppPassword? GetAppPasswordDelegate(App app, AppSecret appSecret, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
