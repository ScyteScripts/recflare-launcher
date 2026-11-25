using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct BytesCount : IValidatedStruct
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

	public static BytesCount operator +(BytesCount a, BytesCount b)
	{
		return new BytesCount(a.Value + b.Value);
	}

	public static BytesCount operator -(BytesCount a, BytesCount b)
	{
		return new BytesCount(a.Value - b.Value);
	}

	public BytesCount(long v)
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

	public static implicit operator long(BytesCount @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
