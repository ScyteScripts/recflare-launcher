using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Updating.Strategy;

public struct UpdateFileState : IValidatedStruct
{
	public AppPath Path { get; }

	public AppVersionId VersionId { get; }

	public BytesCount? Size { get; }

	public Hash? Hash { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, AppVersionId versionId, BytesCount? size, Hash? hash)
	{
		return null;
	}

	public UpdateFileState(AppPath path, AppVersionId versionId, BytesCount? size, Hash? hash)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(versionId, "versionId");
		Arg.IsValid(size, "size");
		Arg.IsValid(hash, "hash");
		string text = Validate(path, versionId, size, hash);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		VersionId = versionId;
		Size = size;
		Hash = hash;
		IsValid = true;
	}

	public UpdateFileState ModifyPath(AppPath newPath)
	{
		return new UpdateFileState(newPath, VersionId, Size, Hash);
	}

	public UpdateFileState ModifyVersionId(AppVersionId newVersionId)
	{
		return new UpdateFileState(Path, newVersionId, Size, Hash);
	}

	public UpdateFileState ModifySize(BytesCount? newSize)
	{
		return new UpdateFileState(Path, VersionId, newSize, Hash);
	}

	public UpdateFileState ModifyHash(Hash? newHash)
	{
		return new UpdateFileState(Path, VersionId, Size, newHash);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"VersionId\" = {VersionId}, \"Size\" = {Size}, \"Hash\" = {Hash}";
		return text + "}";
	}
}
