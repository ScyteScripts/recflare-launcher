using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppAuthKey : IValidatedStruct
{
	public string Value { get; }

	public bool IsValid { get; }

	public static string Validate(string value)
	{
		return null;
	}

	public AppAuthKey(string v)
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

	public static implicit operator string(AppAuthKey @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
