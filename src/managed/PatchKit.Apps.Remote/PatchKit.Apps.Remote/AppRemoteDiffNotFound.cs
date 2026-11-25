using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class AppRemoteDiffNotFound : FunctionException
{
	public AppRemote AppRemote { get; }

	public AppVersionPatchId VersionPatchId { get; }

	public AppRemoteDiffNotFound(AppRemote appRemote, AppVersionPatchId versionPatchId, Exception inner = null)
		: base($"App remote diff with secret {appRemote.Secret} for version patch from {versionPatchId.SourceVersionId} to {versionPatchId.TargetVersionId} not found.", inner)
	{
		AppRemote = appRemote;
		VersionPatchId = versionPatchId;
	}
}
