using System.Threading;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate AppVersionId? GetAppRemoteLowestVersionWithDiffIdDelegate(AppRemote appRemote, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound);
