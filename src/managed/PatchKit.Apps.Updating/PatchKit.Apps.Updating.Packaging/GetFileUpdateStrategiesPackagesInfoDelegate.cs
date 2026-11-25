using System.Threading;
using PatchKit.Apps.Remote;
using PatchKit.Apps.Updating.Strategy;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public delegate ImmutableArray<AppRemotePackageInfo> GetFileUpdateStrategiesPackagesInfoDelegate(AppRemote appRemote, AppVersionId targetVersionId, AppLicenseSecret? licenseSecret, AppPassword? password, ImmutableArray<FileUpdateStrategy> strategies, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionAndReturnDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
