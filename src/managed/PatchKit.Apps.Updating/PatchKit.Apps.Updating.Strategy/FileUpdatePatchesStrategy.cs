using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Strategy;

public struct FileUpdatePatchesStrategy : IValidatedStruct
{
	public AppPath Path { get; }

	public AppVersionId SourceVersionId { get; }

	public BytesCount BytesCount { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, AppVersionId sourceVersionId, BytesCount bytesCount)
	{
		return null;
	}

	public FileUpdatePatchesStrategy(AppPath path, AppVersionId sourceVersionId, BytesCount bytesCount)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(sourceVersionId, "sourceVersionId");
		Arg.IsValid(bytesCount, "bytesCount");
		string text = Validate(path, sourceVersionId, bytesCount);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		SourceVersionId = sourceVersionId;
		BytesCount = bytesCount;
		IsValid = true;
	}

	public FileUpdatePatchesStrategy ModifyPath(AppPath newPath)
	{
		return new FileUpdatePatchesStrategy(newPath, SourceVersionId, BytesCount);
	}

	public FileUpdatePatchesStrategy ModifySourceVersionId(AppVersionId newSourceVersionId)
	{
		return new FileUpdatePatchesStrategy(Path, newSourceVersionId, BytesCount);
	}

	public FileUpdatePatchesStrategy ModifyBytesCount(BytesCount newBytesCount)
	{
		return new FileUpdatePatchesStrategy(Path, SourceVersionId, newBytesCount);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"SourceVersionId\" = {SourceVersionId}, \"BytesCount\" = {BytesCount}";
		return text + "}";
	}
}
