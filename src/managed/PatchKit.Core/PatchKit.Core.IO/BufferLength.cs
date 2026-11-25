using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct BufferLength : IValidatedStruct
{
	public int Value { get; }

	public bool IsValid { get; }

	public static string Validate(int value)
	{
		if (value <= 0)
		{
			return "Value cannot be less or equal to zero.";
		}
		return null;
	}

	public BufferLength(int v)
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

	public static implicit operator int(BufferLength @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
