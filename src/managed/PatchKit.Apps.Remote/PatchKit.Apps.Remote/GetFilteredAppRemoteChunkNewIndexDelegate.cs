using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate long? GetFilteredAppRemoteChunkNewIndexDelegate(long chunkIndex, ImmutableArray<FilteredAppRemotePackageChunksRange> filteredChunksRanges);
