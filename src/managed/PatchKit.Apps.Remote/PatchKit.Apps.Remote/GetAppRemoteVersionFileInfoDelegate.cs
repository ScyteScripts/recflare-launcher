using System.Threading;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate AppRemoteVersionFileInfo? GetAppRemoteVersionFileInfoDelegate(AppRemote appRemote, AppVersionId versionId, AppPath path, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound);
