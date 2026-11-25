using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppVersionPatchId : IValidatedStruct
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

	public AppVersionPatchId(AppVersionId sourceVersionId, AppVersionId targetVersionId)
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

	public AppVersionPatchId ModifySourceVersionId(AppVersionId newSourceVersionId)
	{
		return new AppVersionPatchId(newSourceVersionId, TargetVersionId);
	}

	public AppVersionPatchId ModifyTargetVersionId(AppVersionId newTargetVersionId)
	{
		return new AppVersionPatchId(SourceVersionId, newTargetVersionId);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"SourceVersionId\" = {SourceVersionId}, \"TargetVersionId\" = {TargetVersionId}";
		return text + "}";
	}
}
