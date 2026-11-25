using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Strategy;

public struct FileUpdateDataStrategy : IValidatedStruct
{
	public AppPath Path { get; }

	public BytesCount BytesCount { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, BytesCount bytesCount)
	{
		return null;
	}

	public FileUpdateDataStrategy(AppPath path, BytesCount bytesCount)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(bytesCount, "bytesCount");
		string text = Validate(path, bytesCount);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		BytesCount = bytesCount;
		IsValid = true;
	}

	public FileUpdateDataStrategy ModifyPath(AppPath newPath)
	{
		return new FileUpdateDataStrategy(newPath, BytesCount);
	}

	public FileUpdateDataStrategy ModifyBytesCount(BytesCount newBytesCount)
	{
		return new FileUpdateDataStrategy(Path, newBytesCount);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"BytesCount\" = {BytesCount}";
		return text + "}";
	}
}
