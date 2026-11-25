using System.Threading;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate bool IsAppRemoteLicenseSecretRequiredDelegate(AppRemote appRemote, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<AppRemoteNotFound> onAppRemoteNotFound);
