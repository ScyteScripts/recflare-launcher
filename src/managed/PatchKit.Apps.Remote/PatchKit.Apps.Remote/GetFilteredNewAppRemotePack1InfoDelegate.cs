using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public delegate AppRemotePackageInfo GetFilteredNewAppRemotePack1InfoDelegate(AppRemotePack1UnpackInfo unpackInfo, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo);
