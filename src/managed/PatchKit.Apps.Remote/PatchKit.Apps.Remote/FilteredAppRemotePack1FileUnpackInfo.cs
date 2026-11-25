using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public struct FilteredAppRemotePack1FileUnpackInfo : IValidatedStruct
{
	public AppPath Path { get; }

	public BytesCount Size { get; }

	public BytesCount StartChunkOffset { get; }

	public FilteredAppRemotePackageChunksRange ChunksRange { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path, BytesCount size, BytesCount startChunkOffset, FilteredAppRemotePackageChunksRange chunksRange)
	{
		return null;
	}

	public FilteredAppRemotePack1FileUnpackInfo(AppPath path, BytesCount size, BytesCount startChunkOffset, FilteredAppRemotePackageChunksRange chunksRange)
	{
		this = default;
		Arg.IsValid(path, "path");
		Arg.IsValid(size, "size");
		Arg.IsValid(startChunkOffset, "startChunkOffset");
		Arg.IsValid(chunksRange, "chunksRange");
		string text = Validate(path, size, startChunkOffset, chunksRange);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		Size = size;
		StartChunkOffset = startChunkOffset;
		ChunksRange = chunksRange;
		IsValid = true;
	}

	public FilteredAppRemotePack1FileUnpackInfo ModifyPath(AppPath newPath)
	{
		return new FilteredAppRemotePack1FileUnpackInfo(newPath, Size, StartChunkOffset, ChunksRange);
	}

	public FilteredAppRemotePack1FileUnpackInfo ModifySize(BytesCount newSize)
	{
		return new FilteredAppRemotePack1FileUnpackInfo(Path, newSize, StartChunkOffset, ChunksRange);
	}

	public FilteredAppRemotePack1FileUnpackInfo ModifyStartChunkOffset(BytesCount newStartChunkOffset)
	{
		return new FilteredAppRemotePack1FileUnpackInfo(Path, Size, newStartChunkOffset, ChunksRange);
	}

	public FilteredAppRemotePack1FileUnpackInfo ModifyChunksRange(FilteredAppRemotePackageChunksRange newChunksRange)
	{
		return new FilteredAppRemotePack1FileUnpackInfo(Path, Size, StartChunkOffset, newChunksRange);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}, \"Size\" = {Size}, \"StartChunkOffset\" = {StartChunkOffset}, \"ChunksRange\" = {ChunksRange}";
		return text + "}";
	}
}
