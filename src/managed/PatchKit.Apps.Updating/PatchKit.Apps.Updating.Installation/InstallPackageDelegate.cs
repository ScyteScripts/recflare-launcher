using System.Threading;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public delegate object InstallPackageDelegate(App app, InstallState installState, IPackage package, ReportProgressDelegate<InstallPackageProgress> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
