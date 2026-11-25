using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate AppLicenseSecret? GetAppLicenseSecretDelegate(App app, AppLicenseKey licenseKey, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
