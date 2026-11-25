using System.Threading;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public delegate object ProcessInstallPackagePatchFileTasksQueueDelegate(App app, InstallState installState, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<InconsistentInstallInfo> onInconsistentInstallInfo);
