using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class AppRemoteContentNotFound : FunctionException
{
	public AppRemote AppRemote { get; }

	public AppVersionId VersionId { get; }

	public AppRemoteContentNotFound(AppRemote appRemote, AppVersionId versionId, Exception inner = null)
		: base($"App remote content with secret {appRemote.Secret} for version {versionId} not found.", inner)
	{
		AppRemote = appRemote;
		VersionId = versionId;
	}
}
