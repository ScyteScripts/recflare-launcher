using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate AppEntryInfo? GetAppEntryInfoDelegate(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
