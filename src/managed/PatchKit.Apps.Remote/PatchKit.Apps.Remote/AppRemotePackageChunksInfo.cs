using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Core.IO.Hashing;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackageChunksInfo : IValidatedStruct
{
	public BytesCount Size { get; }

	public ImmutableArray<Hash> Hashes { get; }

	public bool IsValid { get; }

	public static string Validate(BytesCount size, ImmutableArray<Hash> hashes)
	{
		return null;
	}

	public AppRemotePackageChunksInfo(BytesCount size, ImmutableArray<Hash> hashes)
	{
		this = default;
		Arg.IsValid(size, "size");
		Arg.IsValid(hashes, "hashes");
		string text = Validate(size, hashes);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Size = size;
		Hashes = hashes;
		IsValid = true;
	}

	public AppRemotePackageChunksInfo ModifySize(BytesCount newSize)
	{
		return new AppRemotePackageChunksInfo(newSize, Hashes);
	}

	public AppRemotePackageChunksInfo ModifyHashes(ImmutableArray<Hash> newHashes)
	{
		return new AppRemotePackageChunksInfo(Size, newHashes);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Size\" = {Size}, \"Hashes\" = {Hashes}";
		return text + "}";
	}
}
