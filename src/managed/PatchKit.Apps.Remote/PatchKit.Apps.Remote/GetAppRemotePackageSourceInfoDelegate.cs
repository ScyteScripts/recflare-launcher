using PatchKit.Core;
using PatchKit.Core.IO;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public delegate AppRemotePackageSourceInfo GetAppRemotePackageSourceInfoDelegate(HttpAddress address, BytesCount? partSize, BytesCount offset, BytesCount size, Text? authHeaderValue);
