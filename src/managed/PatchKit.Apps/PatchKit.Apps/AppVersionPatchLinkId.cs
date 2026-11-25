using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppVersionPatchLinkId : IValidatedStruct
{
	public AppVersionId SourceVersionId { get; }

	public AppVersionId TargetVersionId { get; }

	public bool IsValid { get; }

	public static string Validate(AppVersionId sourceVersionId, AppVersionId targetVersionId)
	{
		if ((int)targetVersionId <= (int)sourceVersionId)
		{
			return "Source version id cannot be equal or higher to target version id";
		}
		return null;
	}

	public AppVersionPatchLinkId(AppVersionId sourceVersionId, AppVersionId targetVersionId)
	{
		this = default;
		Arg.IsValid(sourceVersionId, "sourceVersionId");
		Arg.IsValid(targetVersionId, "targetVersionId");
		string text = Validate(sourceVersionId, targetVersionId);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		SourceVersionId = sourceVersionId;
		TargetVersionId = targetVersionId;
		IsValid = true;
	}

	public AppVersionPatchLinkId ModifySourceVersionId(AppVersionId newSourceVersionId)
	{
		return new AppVersionPatchLinkId(newSourceVersionId, TargetVersionId);
	}

	public AppVersionPatchLinkId ModifyTargetVersionId(AppVersionId newTargetVersionId)
	{
		return new AppVersionPatchLinkId(SourceVersionId, newTargetVersionId);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"SourceVersionId\" = {SourceVersionId}, \"TargetVersionId\" = {TargetVersionId}";
		return text + "}";
	}
}
