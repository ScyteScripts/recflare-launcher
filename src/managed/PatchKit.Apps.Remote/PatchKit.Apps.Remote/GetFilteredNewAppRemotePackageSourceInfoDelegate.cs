using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate AppRemotePackageSourceInfo GetFilteredNewAppRemotePackageSourceInfoDelegate(AppRemotePackageSourceInfo sourceInfo, ImmutableArray<BytesRange> filteredBytesRanges);
