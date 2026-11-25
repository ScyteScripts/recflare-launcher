using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackageDataFileInfo : IValidatedStruct
{
	public AppPath Path { get; }

	public AppVersionId VersionId { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, AppVersionId versionId)
	{
		return null;
	}

	public AppRemotePackageDataFileInfo(AppPath path, AppVersionId versionId)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(versionId, "versionId");
		string text = Validate(path, versionId);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		VersionId = versionId;
		IsValid = true;
	}

	public AppRemotePackageDataFileInfo ModifyPath(AppPath newPath)
	{
		return new AppRemotePackageDataFileInfo(newPath, VersionId);
	}

	public AppRemotePackageDataFileInfo ModifyVersionId(AppVersionId newVersionId)
	{
		return new AppRemotePackageDataFileInfo(Path, newVersionId);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"VersionId\" = {VersionId}";
		return text + "}";
	}
}
