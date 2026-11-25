using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public struct PackageMultisourceStreamRequest : IValidatedStruct
{
	public ImmutableArray<AppRemotePackageSourceInfo> SourcesInfo { get; }

	public BytesCount Offset { get; }

	public bool IsValid { get; }

	public static string Validate(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, BytesCount offset)
	{
		return null;
	}

	public PackageMultisourceStreamRequest(ImmutableArray<AppRemotePackageSourceInfo> sourcesInfo, BytesCount offset)
	{
		this = default;
		Arg.IsValid(sourcesInfo, "sourcesInfo");
		Arg.IsValid(offset, "offset");
		string text = Validate(sourcesInfo, offset);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		SourcesInfo = sourcesInfo;
		Offset = offset;
		IsValid = true;
	}

	public PackageMultisourceStreamRequest ModifySourcesInfo(ImmutableArray<AppRemotePackageSourceInfo> newSourcesInfo)
	{
		return new PackageMultisourceStreamRequest(newSourcesInfo, Offset);
	}

	public PackageMultisourceStreamRequest ModifyOffset(BytesCount newOffset)
	{
		return new PackageMultisourceStreamRequest(SourcesInfo, newOffset);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"SourcesInfo\" = {SourcesInfo}, \"Offset\" = {Offset}";
		return text + "}";
	}
}
