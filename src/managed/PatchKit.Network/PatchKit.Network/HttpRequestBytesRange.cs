using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public struct HttpRequestBytesRange : IValidatedStruct
{
	public long Start { get; }

	public long? End { get; }

	public bool IsValid { get; }

	public static string Validate(long start, long? end)
	{
		if (start < 0)
		{
			return "Start cannot be less than zero.";
		}
		if (end.HasValue)
		{
			if (end.Value < 0)
			{
				return "End cannot be less than zero.";
			}
			if (end.Value < start)
			{
				return "End cannot be less than Start.";
			}
		}
		return null;
	}

	public HttpRequestBytesRange(long start, long? end)
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

	public HttpRequestBytesRange ModifyStart(long newStart)
	{
		return new HttpRequestBytesRange(newStart, End);
	}

	public HttpRequestBytesRange ModifyEnd(long? newEnd)
	{
		return new HttpRequestBytesRange(Start, newEnd);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Start\" = {Start}, \"End\" = {End}";
		return text + "}";
	}
}
