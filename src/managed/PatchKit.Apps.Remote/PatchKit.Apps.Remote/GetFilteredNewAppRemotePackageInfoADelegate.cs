using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate AppRemotePackageInfo GetFilteredNewAppRemotePackageInfoADelegate(AppRemotePackageInfo info, ImmutableArray<AppRemotePackageFileInfo> newFilesInfo);
