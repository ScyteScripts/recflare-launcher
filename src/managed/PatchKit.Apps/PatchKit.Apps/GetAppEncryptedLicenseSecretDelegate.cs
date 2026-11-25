using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate AppEncryptedLicenseSecret? GetAppEncryptedLicenseSecretDelegate(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
