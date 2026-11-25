using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppVersionId : IValidatedStruct
{
	public int Value { get; }

	public bool IsValid { get; }

	public static string Validate(int value)
	{
		if (value <= 0)
		{
			return "Value must equal to 1 or higher.";
		}
		return null;
	}

	public AppVersionId(int v)
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

	public static implicit operator int(AppVersionId @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
