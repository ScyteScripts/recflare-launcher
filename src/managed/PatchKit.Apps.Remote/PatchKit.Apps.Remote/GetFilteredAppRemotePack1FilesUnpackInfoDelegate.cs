using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate ImmutableArray<FilteredAppRemotePack1FileUnpackInfo> GetFilteredAppRemotePack1FilesUnpackInfoDelegate(ImmutableArray<AppRemotePack1FileUnpackInfo> filesUnpackInfo, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo, BytesCount chunkSize);
