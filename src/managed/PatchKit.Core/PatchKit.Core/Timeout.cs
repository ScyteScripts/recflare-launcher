using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core;

public struct Timeout : IValidatedStruct
{
	public TimeSpan Value { get; }

	public bool IsValid { get; }

	public static string Validate(TimeSpan value)
	{
		if (value.Ticks <= 0)
		{
			return "Value cannot be negative or zero.";
		}
		return null;
	}

	public Timeout(TimeSpan v)
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

	public static implicit operator TimeSpan(Timeout @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
