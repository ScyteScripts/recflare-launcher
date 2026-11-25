using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackageInfo : IValidatedStruct
{
	public ImmutableArray<AppRemotePackageFileInfo> FilesInfo { get; }

	public AppRemotePackageUnpackInfo UnpackInfo { get; }

	public ImmutableArray<AppRemotePackageSourceInfo> SourcesInfo { get; }

	public AppRemotePackageChunksInfo? ChunksInfo { get; }

	public BytesCount Size { get; }

	public bool IsValid { get; }

	public static string Validate(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemotePackageUnpackInfo unpackInfo, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size)
	{
		return null;
	}

	public AppRemotePackageInfo(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemotePackageUnpackInfo unpackInfo, ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, AppRemotePackageChunksInfo? chunksInfo, BytesCount size)
	{
		this = default;
		Arg.IsValid(filesInfo, "filesInfo");
		Arg.IsValid(unpackInfo, "unpackInfo");
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(chunksInfo, "chunksInfo");
		Arg.IsValid(size, "size");
		string text = Validate(filesInfo, unpackInfo, sourcesInfo, chunksInfo, size);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		FilesInfo = filesInfo;
		UnpackInfo = unpackInfo;
		SourcesInfo = sourcesInfo;
		ChunksInfo = chunksInfo;
		Size = size;
		IsValid = true;
	}

	public AppRemotePackageInfo ModifyFilesInfo(ImmutableArray<AppRemotePackageFileInfo> newFilesInfo)
	{
		return new AppRemotePackageInfo(newFilesInfo, UnpackInfo, SourcesInfo, ChunksInfo, Size);
	}

	public AppRemotePackageInfo ModifyUnpackInfo(AppRemotePackageUnpackInfo newUnpackInfo)
	{
		return new AppRemotePackageInfo(FilesInfo, newUnpackInfo, SourcesInfo, ChunksInfo, Size);
	}

	public AppRemotePackageInfo ModifySourcesInfo(ImmutableArray<AppRemotePackageSourceInfo> newSourcesInfo)
	{
		return new AppRemotePackageInfo(FilesInfo, UnpackInfo, newSourcesInfo, ChunksInfo, Size);
	}

	public AppRemotePackageInfo ModifyChunksInfo(AppRemotePackageChunksInfo? newChunksInfo)
	{
		return new AppRemotePackageInfo(FilesInfo, UnpackInfo, SourcesInfo, newChunksInfo, Size);
	}

	public AppRemotePackageInfo ModifySize(BytesCount newSize)
	{
		return new AppRemotePackageInfo(FilesInfo, UnpackInfo, SourcesInfo, ChunksInfo, newSize);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"FilesInfo\" = {FilesInfo}, \"UnpackInfo\" = {UnpackInfo}, \"SourcesInfo\" = {SourcesInfo}, \"ChunksInfo\" = {ChunksInfo}, \"Size\" = {Size}";
		return text + "}";
	}
}
