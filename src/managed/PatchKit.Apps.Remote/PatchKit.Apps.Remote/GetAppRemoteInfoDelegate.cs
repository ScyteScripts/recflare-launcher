using System.Threading;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate AppRemoteInfo GetAppRemoteInfoDelegate(AppRemote appRemote, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound);
