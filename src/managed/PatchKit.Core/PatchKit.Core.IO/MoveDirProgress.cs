using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct MoveDirProgress : IValidatedStruct
{
	public BytesCount TotalBytes { get; }

	public BytesCount MovedBytes { get; }

	public bool IsValid { get; }

	public static string Validate(BytesCount totalBytes, BytesCount movedBytes)
	{
		return null;
	}

	public MoveDirProgress(BytesCount totalBytes, BytesCount movedBytes)
	{
		this = default;
		Arg.IsValid(totalBytes, "totalBytes");
		Arg.IsValid(movedBytes, "movedBytes");
		string text = Validate(totalBytes, movedBytes);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		TotalBytes = totalBytes;
		MovedBytes = movedBytes;
		IsValid = true;
	}

	public MoveDirProgress ModifyTotalBytes(BytesCount newTotalBytes)
	{
		return new MoveDirProgress(newTotalBytes, MovedBytes);
	}

	public MoveDirProgress ModifyMovedBytes(BytesCount newMovedBytes)
	{
		return new MoveDirProgress(TotalBytes, newMovedBytes);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"TotalBytes\" = {TotalBytes}, \"MovedBytes\" = {MovedBytes}";
		return text + "}";
	}
}
