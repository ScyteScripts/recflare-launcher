using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public struct MoveAppProgress : IValidatedStruct
{
	public BytesCount TotalBytes { get; }

	public BytesCount MovedBytes { get; }

	public bool IsValid { get; }

	public static string Validate(BytesCount totalBytes, BytesCount movedBytes)
	{
		return null;
	}

	public MoveAppProgress(BytesCount totalBytes, BytesCount movedBytes)
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

	public MoveAppProgress ModifyTotalBytes(BytesCount newTotalBytes)
	{
		return new MoveAppProgress(newTotalBytes, MovedBytes);
	}

	public MoveAppProgress ModifyMovedBytes(BytesCount newMovedBytes)
	{
		return new MoveAppProgress(TotalBytes, newMovedBytes);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"TotalBytes\" = {TotalBytes}, \"MovedBytes\" = {MovedBytes}";
		return text + "}";
	}
}
