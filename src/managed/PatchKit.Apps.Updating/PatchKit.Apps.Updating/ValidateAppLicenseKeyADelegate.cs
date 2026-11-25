using System.Threading;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public delegate AppLicenseSecret ValidateAppLicenseKeyADelegate(AppSecret appSecret, AppLicenseKey licenseKey, AppLicenseSecret? lastLicenseSecret, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<BlockedAppLicenseKey> onBlockedAppLicenseKey, HandleExceptionAndReturnDelegate<InvalidAppLicenseKey> onInvalidAppLicenseKey);
