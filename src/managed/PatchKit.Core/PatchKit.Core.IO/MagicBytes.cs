using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct MagicBytes : IValidatedStruct
{
	public ImmutableArray<byte> Bytes { get; }

	public BytesCount Offset { get; }

	public bool IsValid { get; }

	public static string Validate(ImmutableArray<byte> bytes, BytesCount offset)
	{
		return null;
	}

	public MagicBytes(ImmutableArray<byte> bytes, BytesCount offset)
	{
		this = default;
		Arg.IsValid(offset, "offset");
		string text = Validate(bytes, offset);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Bytes = bytes;
		Offset = offset;
		IsValid = true;
	}

	public MagicBytes ModifyBytes(ImmutableArray<byte> newBytes)
	{
		return new MagicBytes(newBytes, Offset);
	}

	public MagicBytes ModifyOffset(BytesCount newOffset)
	{
		return new MagicBytes(Bytes, newOffset);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Bytes\" = {Bytes}, \"Offset\" = {Offset}";
		return text + "}";
	}
}
