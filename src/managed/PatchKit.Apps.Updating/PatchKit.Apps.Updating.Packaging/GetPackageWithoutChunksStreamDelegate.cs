using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public delegate IPackageStreamDisposable GetPackageWithoutChunksStreamDelegate(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, BytesCount size);
