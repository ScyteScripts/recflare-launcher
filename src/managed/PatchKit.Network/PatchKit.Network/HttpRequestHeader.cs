using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public struct HttpRequestHeader : IValidatedStruct
{
	public Text Name { get; }

	public Text Value { get; }

	public bool IsValid { get; }

	public static string Validate(Text name, Text value)
	{
		return null;
	}

	public HttpRequestHeader(Text name, Text value)
	{
		this = default;
		Arg.IsValid(name, "name");
		Arg.IsValid(value, "value");
		string text = Validate(name, value);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Name = name;
		Value = value;
		IsValid = true;
	}

	public HttpRequestHeader ModifyName(Text newName)
	{
		return new HttpRequestHeader(newName, Value);
	}

	public HttpRequestHeader ModifyValue(Text newValue)
	{
		return new HttpRequestHeader(Name, newValue);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Name\" = {Name}, \"Value\" = {Value}";
		return text + "}";
	}
}
