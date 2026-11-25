using System.Threading;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public delegate Text GetAppRemotePackageMetaTextDelegate(ImmutableArray<HttpAddress> urls, Text? authHeaderValue, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
