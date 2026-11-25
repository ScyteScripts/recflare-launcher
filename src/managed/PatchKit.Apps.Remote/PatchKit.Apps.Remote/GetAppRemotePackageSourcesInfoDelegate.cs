using PatchKit.Api.Models;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate ImmutableArray<AppRemotePackageSourceInfo> GetAppRemotePackageSourcesInfoDelegate(ImmutableArray<ResourceUrl> resourceUrls, BytesCount size, Text? authHeaderValue);
