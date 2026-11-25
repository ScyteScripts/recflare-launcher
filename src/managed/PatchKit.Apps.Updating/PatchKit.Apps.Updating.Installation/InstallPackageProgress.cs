using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public struct InstallPackageProgress : IValidatedStruct
{
	public BytesCount CurrentFileProcessedBytesCount { get; }

	public bool IsValid { get; }

	public static string Validate(BytesCount currentFileProcessedBytesCount)
	{
		return null;
	}

	public InstallPackageProgress(BytesCount currentFileProcessedBytesCount)
	{
		this = default;
		Arg.IsValid(currentFileProcessedBytesCount, "currentFileProcessedBytesCount");
		string text = Validate(currentFileProcessedBytesCount);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		CurrentFileProcessedBytesCount = currentFileProcessedBytesCount;
		IsValid = true;
	}

	public InstallPackageProgress ModifyCurrentFileProcessedBytesCount(BytesCount newCurrentFileProcessedBytesCount)
	{
		return new InstallPackageProgress(newCurrentFileProcessedBytesCount);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"CurrentFileProcessedBytesCount\" = {CurrentFileProcessedBytesCount}";
		return text + "}";
	}
}
