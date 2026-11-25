using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class AppRemoteVersionPatchLinkNotFound : FunctionException
{
	public AppSecret Secret { get; }

	public AppVersionPatchLinkId VersionPatchLinkId { get; }

	public AppRemoteVersionPatchLinkNotFound(AppSecret secret, AppVersionPatchLinkId versionPatchLinkId, Exception inner = null)
		: base($"App remote with secret {secret} version patch link from {versionPatchLinkId.SourceVersionId} to {versionPatchLinkId.TargetVersionId} not found.", inner)
	{
		Secret = secret;
		VersionPatchLinkId = versionPatchLinkId;
	}
}
