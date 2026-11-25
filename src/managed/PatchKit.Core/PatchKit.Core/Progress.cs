using PatchKit.Core.CSharp;

namespace PatchKit.Core;

public struct Progress : IValidatedStruct
{
	public double Percentage { get; }

	public bool IsValid { get; }

	public static string Validate(double percentage)
	{
		if (percentage < 0.0 || percentage > 1.0)
		{
			return "Percentage cannot be outside of 0.0 - 1.0 range.";
		}
		return null;
	}

	public Progress(double percentage)
	{
		this = default;
		string text = Validate(percentage);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Percentage = percentage;
		IsValid = true;
	}

	public Progress ModifyPercentage(double newPercentage)
	{
		return new Progress(newPercentage);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Percentage\" = {Percentage}";
		return text + "}";
	}
}
