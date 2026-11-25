using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppEntryInfo : IValidatedStruct
{
	public AppPath Path { get; }

	public AppEntryType Type { get; }

	public AppVersionId VersionId { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, AppEntryType type, AppVersionId versionId)
	{
		return null;
	}

	public AppEntryInfo(AppPath path, AppEntryType type, AppVersionId versionId)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(versionId, "versionId");
		string text = Validate(path, type, versionId);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		Type = type;
		VersionId = versionId;
		IsValid = true;
	}

	public AppEntryInfo ModifyPath(AppPath newPath)
	{
		return new AppEntryInfo(newPath, Type, VersionId);
	}

	public AppEntryInfo ModifyType(AppEntryType newType)
	{
		return new AppEntryInfo(Path, newType, VersionId);
	}

	public AppEntryInfo ModifyVersionId(AppVersionId newVersionId)
	{
		return new AppEntryInfo(Path, Type, newVersionId);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"Type\" = {Type}, \"VersionId\" = {VersionId}";
		return text + "}";
	}
}
