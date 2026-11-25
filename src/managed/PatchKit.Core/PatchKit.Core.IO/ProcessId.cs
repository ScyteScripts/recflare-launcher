using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct ProcessId : IValidatedStruct
{
	public int Value { get; }

	public bool IsValid { get; }

	public static string Validate(int value)
	{
		return null;
	}

	public ProcessId(int v)
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

	public static implicit operator int(ProcessId @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
