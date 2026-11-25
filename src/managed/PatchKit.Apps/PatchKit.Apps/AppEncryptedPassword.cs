using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppEncryptedPassword : IValidatedStruct
{
	public string Value { get; }

	public bool IsValid { get; }

	public static string Validate(string value)
	{
		return null;
	}

	public AppEncryptedPassword(string v)
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

	public static implicit operator string(AppEncryptedPassword @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
