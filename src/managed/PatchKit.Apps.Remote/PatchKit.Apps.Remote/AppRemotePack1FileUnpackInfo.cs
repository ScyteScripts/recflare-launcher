using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public struct AppRemotePack1FileUnpackInfo : IValidatedStruct
{
	public AppPath Path { get; }

	public BytesCount Offset { get; }

	public BytesCount Size { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, BytesCount offset, BytesCount size)
	{
		return null;
	}

	public AppRemotePack1FileUnpackInfo(AppPath path, BytesCount offset, BytesCount size)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(offset, "offset");
		Arg.IsValid(size, "size");
		string text = Validate(path, offset, size);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		Offset = offset;
		Size = size;
		IsValid = true;
	}

	public AppRemotePack1FileUnpackInfo ModifyPath(AppPath newPath)
	{
		return new AppRemotePack1FileUnpackInfo(newPath, Offset, Size);
	}

	public AppRemotePack1FileUnpackInfo ModifyOffset(BytesCount newOffset)
	{
		return new AppRemotePack1FileUnpackInfo(Path, newOffset, Size);
	}

	public AppRemotePack1FileUnpackInfo ModifySize(BytesCount newSize)
	{
		return new AppRemotePack1FileUnpackInfo(Path, Offset, newSize);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"Offset\" = {Offset}, \"Size\" = {Size}";
		return text + "}";
	}
}
