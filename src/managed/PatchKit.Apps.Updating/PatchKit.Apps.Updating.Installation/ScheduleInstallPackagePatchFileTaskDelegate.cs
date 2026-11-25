using System.Threading;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public delegate object ScheduleInstallPackagePatchFileTaskDelegate(App app, InstallState installState, IPackagePatchFile file, ReportProgressDelegate<object> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
