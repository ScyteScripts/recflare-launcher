using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackageSourceInfo : IValidatedStruct
{
	public ImmutableArray<AppRemotePackageSourcePartInfo> PartsInfo { get; }

	public bool IsValid { get; }

	public static string Validate(ImmutableArray<AppRemotePackageSourcePartInfo> partsInfo)
	{
		return null;
	}

	public AppRemotePackageSourceInfo(ImmutableArray<AppRemotePackageSourcePartInfo> partsInfo)
	{
		this = default;
		Arg.IsValid(partsInfo, "partsInfo");
		string text = Validate(partsInfo);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		PartsInfo = partsInfo;
		IsValid = true;
	}

	public AppRemotePackageSourceInfo ModifyPartsInfo(ImmutableArray<AppRemotePackageSourcePartInfo> newPartsInfo)
	{
		return new AppRemotePackageSourceInfo(newPartsInfo);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"PartsInfo\" = {PartsInfo}";
		return text + "}";
	}
}
