using System.Threading;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public delegate AppRemotePack1UnpackInfo GetAppRemotePack1UnpackInfoBDelegate(ImmutableArray<HttpAddress> metaUrls, AppRemotePack1UnpackPassword password, Text? authHeaderValue, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
