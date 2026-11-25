using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public struct PackageSourcePartStreamRequest : IValidatedStruct
{
	public AppRemotePackageSourcePartInfo Info { get; }

	public BytesCount Offset { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemotePackageSourcePartInfo info, BytesCount offset)
	{
		return null;
	}

	public PackageSourcePartStreamRequest(AppRemotePackageSourcePartInfo info, BytesCount offset)
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

	public PackageSourcePartStreamRequest ModifyInfo(AppRemotePackageSourcePartInfo newInfo)
	{
		return new PackageSourcePartStreamRequest(newInfo, Offset);
	}

	public PackageSourcePartStreamRequest ModifyOffset(BytesCount newOffset)
	{
		return new PackageSourcePartStreamRequest(Info, newOffset);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Info\" = {Info}, \"Offset\" = {Offset}";
		return text + "}";
	}
}
