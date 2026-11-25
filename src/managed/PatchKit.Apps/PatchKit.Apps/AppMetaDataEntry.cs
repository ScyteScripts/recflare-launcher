using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppMetaDataEntry : IValidatedStruct
{
	public AppPath Path { get; }

	public AppVersionId VersionId { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, AppVersionId versionId)
	{
		return null;
	}

	public AppMetaDataEntry(AppPath path, AppVersionId versionId)
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

	public AppMetaDataEntry ModifyPath(AppPath newPath)
	{
		return new AppMetaDataEntry(newPath, VersionId);
	}

	public AppMetaDataEntry ModifyVersionId(AppVersionId newVersionId)
	{
		return new AppMetaDataEntry(Path, newVersionId);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"VersionId\" = {VersionId}";
		return text + "}";
	}
}
