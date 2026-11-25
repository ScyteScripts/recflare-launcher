using System.Collections.Generic;
using System.Linq;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO.Hashing;

public struct Hash : IValidatedStruct
{
	public string BytesAsString
	{
		get
		{
			IEnumerable<string> source = Value.Select((byte b) => b.ToString("X2")).Reverse();
			return string.Join("", source.ToArray()).ToLower().TrimStart(new char[1] { '0' });
		}
	}

	public ImmutableArray<byte> Value { get; }

	public bool IsValid { get; }

	public static string Validate(ImmutableArray<byte> value)
	{
		return null;
	}

	public Hash(ImmutableArray<byte> v)
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

	public static implicit operator ImmutableArray<byte>(Hash @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
