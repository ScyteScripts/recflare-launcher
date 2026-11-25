using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object SetAppLicenseKeyAndSecretDelegate(App app, AppLicenseKey licenseKey, AppLicenseSecret licenseSecret, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
