using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate ImmutableArray<AppRemotePackageSourcePartInfo> GetFilteredNewAppRemotePackageSourcePartsInfoForBytesRangeDelegate(ImmutableArray<AppRemotePackageSourcePartInfo> partsInfo, BytesRange filteredBytesRange);
