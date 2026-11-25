using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackageUnpackInfo : IValidatedStruct
{
	public AppRemotePack1UnpackInfo? Pack1Info { get; }

	public AppRemoteZipUnpackInfo? ZipInfo { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemotePack1UnpackInfo? pack1Info, AppRemoteZipUnpackInfo? zipInfo)
	{
		if (pack1Info.HasValue && zipInfo.HasValue)
		{
			return "Only one value can be provided.";
		}
		if (!pack1Info.HasValue && !zipInfo.HasValue)
		{
			return "At least one value must be provided.";
		}
		return null;
	}

	public static implicit operator AppRemotePackageUnpackInfo(AppRemotePack1UnpackInfo value)
	{
		return new AppRemotePackageUnpackInfo(value, null);
	}

	public static implicit operator AppRemotePackageUnpackInfo(AppRemoteZipUnpackInfo value)
	{
		return new AppRemotePackageUnpackInfo(null, value);
	}

	public AppRemotePackageUnpackInfo(AppRemotePack1UnpackInfo? pack1Info, AppRemoteZipUnpackInfo? zipInfo)
	{
		this = default;
		Arg.IsValid(pack1Info, "pack1Info");
		Arg.IsValid(zipInfo, "zipInfo");
		string text = Validate(pack1Info, zipInfo);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Pack1Info = pack1Info;
		ZipInfo = zipInfo;
		IsValid = true;
	}

	public AppRemotePackageUnpackInfo ModifyPack1Info(AppRemotePack1UnpackInfo? newPack1Info)
	{
		return new AppRemotePackageUnpackInfo(newPack1Info, ZipInfo);
	}

	public AppRemotePackageUnpackInfo ModifyZipInfo(AppRemoteZipUnpackInfo? newZipInfo)
	{
		return new AppRemotePackageUnpackInfo(Pack1Info, newZipInfo);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Pack1Info\" = {Pack1Info}, \"ZipInfo\" = {ZipInfo}";
		return text + "}";
	}
}
