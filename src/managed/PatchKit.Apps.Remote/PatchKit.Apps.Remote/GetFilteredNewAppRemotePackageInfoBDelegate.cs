using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public delegate AppRemotePackageInfo GetFilteredNewAppRemotePackageInfoBDelegate(AppRemotePackageInfo info, ImmutableArray<AppPath> dataFiles, ImmutableArray<AppPath> patchFiles);
