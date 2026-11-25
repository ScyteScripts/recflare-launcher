using System.Threading;
using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Strategy;

public delegate ImmutableArray<AppPath> GetEntriesToDeleteDelegate(UpdateAppContext context, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound);
