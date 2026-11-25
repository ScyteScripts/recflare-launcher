using System.Threading;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate ImmutableArray<AppPath> GetAppRemoteVersionDirPathsDelegate(AppRemote appRemote, AppVersionId versionId, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound);
