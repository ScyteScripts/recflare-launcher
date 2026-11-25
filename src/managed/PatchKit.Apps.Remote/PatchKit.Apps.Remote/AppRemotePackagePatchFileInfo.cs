using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackagePatchFileInfo : IValidatedStruct
{
	public AppPath Path { get; }

	public AppVersionId SourceVersionId { get; }

	public AppVersionId TargetVersionId { get; }

	public bool IsPatchEmpty { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, AppVersionId sourceVersionId, AppVersionId targetVersionId, bool isPatchEmpty)
	{
		if ((int)sourceVersionId > (int)targetVersionId)
		{
			return "Source version id cannot be higher than target version id.";
		}
		return null;
	}

	public AppRemotePackagePatchFileInfo(AppPath path, AppVersionId sourceVersionId, AppVersionId targetVersionId, bool isPatchEmpty)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(sourceVersionId, "sourceVersionId");
		Arg.IsValid(targetVersionId, "targetVersionId");
		string text = Validate(path, sourceVersionId, targetVersionId, isPatchEmpty);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		SourceVersionId = sourceVersionId;
		TargetVersionId = targetVersionId;
		IsPatchEmpty = isPatchEmpty;
		IsValid = true;
	}

	public AppRemotePackagePatchFileInfo ModifyPath(AppPath newPath)
	{
		return new AppRemotePackagePatchFileInfo(newPath, SourceVersionId, TargetVersionId, IsPatchEmpty);
	}

	public AppRemotePackagePatchFileInfo ModifySourceVersionId(AppVersionId newSourceVersionId)
	{
		return new AppRemotePackagePatchFileInfo(Path, newSourceVersionId, TargetVersionId, IsPatchEmpty);
	}

	public AppRemotePackagePatchFileInfo ModifyTargetVersionId(AppVersionId newTargetVersionId)
	{
		return new AppRemotePackagePatchFileInfo(Path, SourceVersionId, newTargetVersionId, IsPatchEmpty);
	}

	public AppRemotePackagePatchFileInfo ModifyIsPatchEmpty(bool newIsPatchEmpty)
	{
		return new AppRemotePackagePatchFileInfo(Path, SourceVersionId, TargetVersionId, newIsPatchEmpty);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"SourceVersionId\" = {SourceVersionId}, \"TargetVersionId\" = {TargetVersionId}, \"IsPatchEmpty\" = {IsPatchEmpty}";
		return text + "}";
	}
}
