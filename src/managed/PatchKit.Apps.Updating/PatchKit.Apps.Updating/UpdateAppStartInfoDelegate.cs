using System.Threading;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public delegate object UpdateAppStartInfoDelegate(UpdateAppContext context, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound);
