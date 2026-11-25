using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemoteZipUnpackPassword : IValidatedStruct
{
	public string Value { get; }

	public bool IsValid { get; }

	public static string Validate(string value)
	{
		if (value == string.Empty)
		{
			return "Value cannot be empty.";
		}
		return null;
	}

	public AppRemoteZipUnpackPassword(string v)
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

	public static implicit operator string(AppRemoteZipUnpackPassword @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
