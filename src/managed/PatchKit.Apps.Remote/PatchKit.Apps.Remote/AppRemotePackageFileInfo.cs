using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackageFileInfo : IValidatedStruct
{
	public AppPath Path
	{
		get
		{
			if (DataFileInfo.HasValue)
			{
				return DataFileInfo.Value.Path;
			}
			if (PatchFileInfo.HasValue)
			{
				return PatchFileInfo.Value.Path;
			}
			throw new UnpredictedCondition("Invalid values");
		}
	}

	public AppRemotePackageDataFileInfo? DataFileInfo { get; }

	public AppRemotePackagePatchFileInfo? PatchFileInfo { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemotePackageDataFileInfo? dataFileInfo, AppRemotePackagePatchFileInfo? patchFileInfo)
	{
		if (!dataFileInfo.HasValue && !patchFileInfo.HasValue)
		{
			return "Must contain at least one value.";
		}
		if (dataFileInfo.HasValue && patchFileInfo.HasValue)
		{
			return "Cannot contain both values.";
		}
		return null;
	}

	public static implicit operator AppRemotePackageFileInfo(AppRemotePackageDataFileInfo value)
	{
		return new AppRemotePackageFileInfo(value, null);
	}

	public static implicit operator AppRemotePackageFileInfo(AppRemotePackagePatchFileInfo value)
	{
		return new AppRemotePackageFileInfo(null, value);
	}

	public AppRemotePackageFileInfo(AppRemotePackageDataFileInfo? dataFileInfo, AppRemotePackagePatchFileInfo? patchFileInfo)
	{
		this = default;
		Arg.IsValid(dataFileInfo, "dataFileInfo");
		Arg.IsValid(patchFileInfo, "patchFileInfo");
		string text = Validate(dataFileInfo, patchFileInfo);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		DataFileInfo = dataFileInfo;
		PatchFileInfo = patchFileInfo;
		IsValid = true;
	}

	public AppRemotePackageFileInfo ModifyDataFileInfo(AppRemotePackageDataFileInfo? newDataFileInfo)
	{
		return new AppRemotePackageFileInfo(newDataFileInfo, PatchFileInfo);
	}

	public AppRemotePackageFileInfo ModifyPatchFileInfo(AppRemotePackagePatchFileInfo? newPatchFileInfo)
	{
		return new AppRemotePackageFileInfo(DataFileInfo, newPatchFileInfo);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"DataFileInfo\" = {DataFileInfo}, \"PatchFileInfo\" = {PatchFileInfo}";
		return text + "}";
	}
}
