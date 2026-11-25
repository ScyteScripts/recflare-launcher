using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public struct PackageSourceStreamRequest : IValidatedStruct
{
	public AppRemotePackageSourceInfo Info { get; }

	public BytesCount Offset { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemotePackageSourceInfo info, BytesCount offset)
	{
		return null;
	}

	public PackageSourceStreamRequest(AppRemotePackageSourceInfo info, BytesCount offset)
	{
		this = default;
		Arg.IsValid(info, "info");
		Arg.IsValid(offset, "offset");
		string text = Validate(info, offset);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Info = info;
		Offset = offset;
		IsValid = true;
	}

	public PackageSourceStreamRequest ModifyInfo(AppRemotePackageSourceInfo newInfo)
	{
		return new PackageSourceStreamRequest(newInfo, Offset);
	}

	public PackageSourceStreamRequest ModifyOffset(BytesCount newOffset)
	{
		return new PackageSourceStreamRequest(Info, newOffset);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Info\" = {Info}, \"Offset\" = {Offset}";
		return text + "}";
	}
}
