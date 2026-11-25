using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct StreamLength : IValidatedStruct
{
	public long Value { get; }

	public bool IsValid { get; }

	public static string Validate(long value)
	{
		if (value < 0)
		{
			return "Value cannot be less than zero.";
		}
		return null;
	}

	public StreamLength(long v)
	{
		this = default;
		string text = Validate(v);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Value = v;
		IsValid = true;
	}

	public static implicit operator long(StreamLength @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
