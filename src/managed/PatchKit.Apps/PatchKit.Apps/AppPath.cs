using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public struct AppPath : IValidatedStruct
{
	public string Value { get; }

	public bool IsValid { get; }

	public static string Validate(string value)
	{
		string text = Path.Validate(value);
		if (text != null)
		{
			return text;
		}
		if (value.StartsWith("/"))
		{
			return "Value cannot start with a slash.";
		}
		if (value.Contains("../"))
		{
			return "Value cannot contain parent directory redirections.";
		}
		return null;
	}

	public AppPath(string v)
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

	public static implicit operator string(AppPath @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
