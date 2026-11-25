using System.Collections.Generic;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public sealed class AppRemote
{
	public AppSecret Secret { get; }

	public AppAuthKey? AuthKey { get; set; }

	public AppRemoteInfo? InfoCache { get; set; }

	public Dictionary<AppVersionId, AppRemotePackageInfo> ContentInfoCache { get; } = new Dictionary<AppVersionId, AppRemotePackageInfo>();

	public Dictionary<AppVersionPatchLinkId, AppRemotePackageInfo> DiffInfoCache { get; } = new Dictionary<AppVersionPatchLinkId, AppRemotePackageInfo>();

	public Dictionary<AppVersionId, ImmutableArray<AppRemoteVersionEntryInfo>> VersionEntriesInfoCache { get; } = new Dictionary<AppVersionId, ImmutableArray<AppRemoteVersionEntryInfo>>();

	public Dictionary<AppVersionId, ImmutableArray<AppRemoteVersionFileInfo>> VersionFilesInfoCache { get; set; } = new Dictionary<AppVersionId, ImmutableArray<AppRemoteVersionFileInfo>>();

	public Dictionary<AppVersionId, ImmutableArray<AppRemoteVersionDirInfo>> VersionDirsInfoCache { get; } = new Dictionary<AppVersionId, ImmutableArray<AppRemoteVersionDirInfo>>();

	public Dictionary<AppVersionId, AppRemoteVersionMetaInfo> VersionMetaInfoCache { get; set; } = new Dictionary<AppVersionId, AppRemoteVersionMetaInfo>();

	public AppRemote(AppSecret secret, AppAuthKey? authKey)
	{
		Secret = secret;
		AuthKey = authKey;
	}
}
