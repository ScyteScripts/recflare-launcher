using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct Path : IValidatedStruct
{
	public string Value { get; }

	public bool IsValid { get; }

	public static string Validate(string value)
	{
		if (value == string.Empty)
		{
			return "Value cannot empty.";
		}
		if (value.Contains("\\"))
		{
			return "Value cannot contain backward slashes.";
		}
		if (value.Contains("//"))
		{
			return "Value cannot contain multiple forward slashes, placed next to each other.";
		}
		if (value.EndsWith("/"))
		{
			return "Value cannot end with a slash.";
		}
		return null;
	}

	public Path(string v)
	{
		this = default;
		Arg.IsNotNull(v, "v");
		string text = Validate(v);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Value = v;
		IsValid = true;
	}

	public static implicit operator string(Path @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
