using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public struct PackageSourceStreamContext : IValidatedStruct
{
	public AppRemotePackageSourceInfo Info { get; }

	public BytesCount Position { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemotePackageSourceInfo info, BytesCount position)
	{
		return null;
	}

	public PackageSourceStreamContext(AppRemotePackageSourceInfo info, BytesCount position)
	{
		this = default;
		Arg.IsValid(info, "info");
		Arg.IsValid(position, "position");
		string text = Validate(info, position);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Info = info;
		Position = position;
		IsValid = true;
	}

	public PackageSourceStreamContext ModifyInfo(AppRemotePackageSourceInfo newInfo)
	{
		return new PackageSourceStreamContext(newInfo, Position);
	}

	public PackageSourceStreamContext ModifyPosition(BytesCount newPosition)
	{
		return new PackageSourceStreamContext(Info, newPosition);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Info\" = {Info}, \"Position\" = {Position}";
		return text + "}";
	}
}
