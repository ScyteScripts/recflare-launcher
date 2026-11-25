using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemoteInfo : IValidatedStruct
{
	public Text? Name { get; }

	public bool IsLicenseSecretRequired { get; }

	public bool IsAuthTokenRequired { get; }

	public bool IsPasswordRequired { get; }

	public AppVersionId LatestVersionId { get; }

	public AppVersionId? LowestVersionWithDiffId { get; }

	public AppRemotePatcherInfo PatcherInfo { get; }

	public bool IsValid { get; }

	public static string Validate(Text? name, bool isLicenseSecretRequired, bool isAuthTokenRequired, bool isPasswordRequired, AppVersionId latestVersionId, AppVersionId? lowestVersionWithDiffId, AppRemotePatcherInfo patcherInfo)
	{
		if (lowestVersionWithDiffId.HasValue && (int)lowestVersionWithDiffId.Value > (int)latestVersionId)
		{
			return "Lowest version with diff id cannot be higher that latest version id";
		}
		return null;
	}

	public AppRemoteInfo(Text? name, bool isLicenseSecretRequired, bool isAuthTokenRequired, bool isPasswordRequired, AppVersionId latestVersionId, AppVersionId? lowestVersionWithDiffId, AppRemotePatcherInfo patcherInfo)
	{
		this = default;
		Arg.IsValid(name, "name");
		Arg.IsValid(latestVersionId, "latestVersionId");
		Arg.IsValid(lowestVersionWithDiffId, "lowestVersionWithDiffId");
		Arg.IsValid(patcherInfo, "patcherInfo");
		string text = Validate(name, isLicenseSecretRequired, isAuthTokenRequired, isPasswordRequired, latestVersionId, lowestVersionWithDiffId, patcherInfo);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Name = name;
		IsLicenseSecretRequired = isLicenseSecretRequired;
		IsAuthTokenRequired = isAuthTokenRequired;
		IsPasswordRequired = isPasswordRequired;
		LatestVersionId = latestVersionId;
		LowestVersionWithDiffId = lowestVersionWithDiffId;
		PatcherInfo = patcherInfo;
		IsValid = true;
	}

	public AppRemoteInfo ModifyName(Text? newName)
	{
		return new AppRemoteInfo(newName, IsLicenseSecretRequired, IsAuthTokenRequired, IsPasswordRequired, LatestVersionId, LowestVersionWithDiffId, PatcherInfo);
	}

	public AppRemoteInfo ModifyIsLicenseSecretRequired(bool newIsLicenseSecretRequired)
	{
		return new AppRemoteInfo(Name, newIsLicenseSecretRequired, IsAuthTokenRequired, IsPasswordRequired, LatestVersionId, LowestVersionWithDiffId, PatcherInfo);
	}

	public AppRemoteInfo ModifyIsAuthTokenRequired(bool newIsAuthTokenRequired)
	{
		return new AppRemoteInfo(Name, IsLicenseSecretRequired, newIsAuthTokenRequired, IsPasswordRequired, LatestVersionId, LowestVersionWithDiffId, PatcherInfo);
	}

	public AppRemoteInfo ModifyIsPasswordRequired(bool newIsPasswordRequired)
	{
		return new AppRemoteInfo(Name, IsLicenseSecretRequired, IsAuthTokenRequired, newIsPasswordRequired, LatestVersionId, LowestVersionWithDiffId, PatcherInfo);
	}

	public AppRemoteInfo ModifyLatestVersionId(AppVersionId newLatestVersionId)
	{
		return new AppRemoteInfo(Name, IsLicenseSecretRequired, IsAuthTokenRequired, IsPasswordRequired, newLatestVersionId, LowestVersionWithDiffId, PatcherInfo);
	}

	public AppRemoteInfo ModifyLowestVersionWithDiffId(AppVersionId? newLowestVersionWithDiffId)
	{
		return new AppRemoteInfo(Name, IsLicenseSecretRequired, IsAuthTokenRequired, IsPasswordRequired, LatestVersionId, newLowestVersionWithDiffId, PatcherInfo);
	}

	public AppRemoteInfo ModifyPatcherInfo(AppRemotePatcherInfo newPatcherInfo)
	{
		return new AppRemoteInfo(Name, IsLicenseSecretRequired, IsAuthTokenRequired, IsPasswordRequired, LatestVersionId, LowestVersionWithDiffId, newPatcherInfo);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Name\" = {Name}, \"IsLicenseSecretRequired\" = {IsLicenseSecretRequired}, \"IsAuthTokenRequired\" = {IsAuthTokenRequired}, \"IsPasswordRequired\" = {IsPasswordRequired}, \"LatestVersionId\" = {LatestVersionId}, \"LowestVersionWithDiffId\" = {LowestVersionWithDiffId}, \"PatcherInfo\" = {PatcherInfo}";
		return text + "}";
	}
}
