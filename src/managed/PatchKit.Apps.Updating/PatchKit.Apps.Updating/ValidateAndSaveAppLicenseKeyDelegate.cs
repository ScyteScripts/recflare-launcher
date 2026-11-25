using System.Threading;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public delegate AppLicenseSecret ValidateAndSaveAppLicenseKeyDelegate(App app, AppSecret appSecret, AppLicenseKey licenseKey, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<BlockedAppLicenseKey> onBlockedAppLicenseKey, HandleExceptionAndReturnDelegate<InvalidAppLicenseKey> onInvalidAppLicenseKey, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
