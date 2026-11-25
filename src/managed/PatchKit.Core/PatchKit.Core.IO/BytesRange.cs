using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct BytesRange : IValidatedStruct
{
	public long Start { get; }

	public long End { get; }

	public bool IsValid { get; }

	public static string Validate(long start, long end)
	{
		if (start < 0)
		{
			return "Start cannot be less than zero.";
		}
		if (end < 0)
		{
			return "End cannot be less than zero.";
		}
		if (end < start)
		{
			return "End cannot be less than Start.";
		}
		return null;
	}

	public BytesRange(long start, long end)
	{
		this = default;
		string text = Validate(start, end);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Start = start;
		End = end;
		IsValid = true;
	}

	public BytesRange ModifyStart(long newStart)
	{
		return new BytesRange(newStart, End);
	}

	public BytesRange ModifyEnd(long newEnd)
	{
		return new BytesRange(Start, newEnd);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Start\" = {Start}, \"End\" = {End}";
		return text + "}";
	}
}
