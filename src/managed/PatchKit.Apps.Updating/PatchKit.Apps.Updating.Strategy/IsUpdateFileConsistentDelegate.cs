using System.Threading;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Strategy;

public delegate bool IsUpdateFileConsistentDelegate(AppRemote appRemote, UpdateFileState state, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound);
