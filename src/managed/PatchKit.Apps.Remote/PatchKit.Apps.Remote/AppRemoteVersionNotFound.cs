using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class AppRemoteVersionNotFound : FunctionException
{
	public AppSecret Secret { get; }

	public AppVersionId VersionId { get; }

	public AppRemoteVersionNotFound(AppSecret secret, AppVersionId versionId, Exception inner = null)
		: base($"App remote with secret {secret} version with id {versionId} not found.", inner)
	{
		Secret = secret;
		VersionId = versionId;
	}
}
