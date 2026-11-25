using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct StreamReadResult : IValidatedStruct
{
	public static readonly StreamReadResult EndOfStream = new StreamReadResult(new StreamReadLength(0));

	public bool IsNotEndOfStream => !IsEndOfStream;

	public bool IsEndOfStream => (int)Length == 0;

	public StreamReadLength Length { get; }

	public bool IsValid { get; }

	public static string Validate(StreamReadLength length)
	{
		return null;
	}

	public StreamReadResult(StreamReadLength length)
	{
		this = default;
		Arg.IsValid(length, "length");
		string text = Validate(length);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Length = length;
		IsValid = true;
	}

	public StreamReadResult ModifyLength(StreamReadLength newLength)
	{
		return new StreamReadResult(newLength);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Length\" = {Length}";
		return text + "}";
	}
}
