using PatchKit.Core.CSharp;

namespace PatchKit.Core;

public struct Text : IValidatedStruct
{
	public static readonly Text Empty = new Text(string.Empty);

	public bool IsEmpty => Value == string.Empty;

	public string Value { get; }

	public bool IsValid { get; }

	public static string Validate(string value)
	{
		return null;
	}

	public Text(string v)
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

	public static implicit operator string(Text @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
