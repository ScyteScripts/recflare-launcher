using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate AppLicenseKey? GetAppLicenseKeyDelegate(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
