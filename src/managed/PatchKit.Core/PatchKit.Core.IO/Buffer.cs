using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public struct Buffer<T> : IValidatedStruct
{
	public T[] Array { get; }

	public BufferOffset Offset { get; }

	public BufferLength Length { get; }

	public bool IsValid { get; }

	public static string Validate(T[] array, BufferOffset offset, BufferLength length)
	{
		if (array.Length - offset.Value < length.Value)
		{
			return "Offset and Length mark unreachable range inside Array.";
		}
		return null;
	}

	public static Buffer<T> CreateOfLength(BufferLength length)
	{
		Assert.IsValid(length);
		return new Buffer<T>(new T[(int)length], new BufferOffset(0), length);
	}

	public Buffer(T[] array, BufferOffset offset, BufferLength length)
	{
		this = default;
		Arg.IsNotNull(array, "array");
		Arg.IsValid(offset, "offset");
		Arg.IsValid(length, "length");
		string text = Validate(array, offset, length);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Array = array;
		Offset = offset;
		Length = length;
		IsValid = true;
	}

	public Buffer<T> ModifyArray(T[] newArray)
	{
		return new Buffer<T>(newArray, Offset, Length);
	}

	public Buffer<T> ModifyOffset(BufferOffset newOffset)
	{
		return new Buffer<T>(Array, newOffset, Length);
	}

	public Buffer<T> ModifyLength(BufferLength newLength)
	{
		return new Buffer<T>(Array, Offset, newLength);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Array\" = {Array}, \"Offset\" = {Offset}, \"Length\" = {Length}";
		return text + "}";
	}
}
