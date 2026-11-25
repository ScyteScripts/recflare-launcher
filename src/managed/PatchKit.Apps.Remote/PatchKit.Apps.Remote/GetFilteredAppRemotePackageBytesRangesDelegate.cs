using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate ImmutableArray<BytesRange> GetFilteredAppRemotePackageBytesRangesDelegate(ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges, BytesCount chunkSize);
