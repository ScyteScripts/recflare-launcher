using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate ImmutableArray<AppRemotePackageSourceInfo> GetFilteredNewAppRemotePackageSourcesInfoDelegate(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, ImmutableArray<BytesRange> filteredBytesRanges);
