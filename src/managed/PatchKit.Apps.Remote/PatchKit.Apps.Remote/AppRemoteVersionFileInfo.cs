using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Remote;

public struct AppRemoteVersionFileInfo : IValidatedStruct
{
	public AppPath Path { get; }

	public Hash Hash { get; }

	public BytesCount Size { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, Hash hash, BytesCount size)
	{
		return null;
	}

	public AppRemoteVersionFileInfo(AppPath path, Hash hash, BytesCount size)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(hash, "hash");
		Arg.IsValid(size, "size");
		string text = Validate(path, hash, size);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		Hash = hash;
		Size = size;
		IsValid = true;
	}

	public AppRemoteVersionFileInfo ModifyPath(AppPath newPath)
	{
		return new AppRemoteVersionFileInfo(newPath, Hash, Size);
	}

	public AppRemoteVersionFileInfo ModifyHash(Hash newHash)
	{
		return new AppRemoteVersionFileInfo(Path, newHash, Size);
	}

	public AppRemoteVersionFileInfo ModifySize(BytesCount newSize)
	{
		return new AppRemoteVersionFileInfo(Path, Hash, newSize);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"Hash\" = {Hash}, \"Size\" = {Size}";
		return text + "}";
	}
}
