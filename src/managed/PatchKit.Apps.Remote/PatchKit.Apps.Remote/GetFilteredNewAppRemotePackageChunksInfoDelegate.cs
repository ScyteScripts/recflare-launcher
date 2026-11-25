using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate AppRemotePackageChunksInfo? GetFilteredNewAppRemotePackageChunksInfoDelegate(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, AppRemotePackageChunksInfo? info);
