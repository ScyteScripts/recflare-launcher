using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePatcherInfo : IValidatedStruct
{
	public AppRemotePatcherBannerInfo? BannerInfo { get; }

	public Text DownloadSpeedUnit { get; }

	public bool Whitelabel { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemotePatcherBannerInfo? bannerInfo, Text downloadSpeedUnit, bool whitelabel)
	{
		return null;
	}

	public AppRemotePatcherInfo(AppRemotePatcherBannerInfo? bannerInfo, Text downloadSpeedUnit, bool whitelabel)
	{
		this = default;
		Arg.IsValid(bannerInfo, "bannerInfo");
		Arg.IsValid(downloadSpeedUnit, "downloadSpeedUnit");
		string text = Validate(bannerInfo, downloadSpeedUnit, whitelabel);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		BannerInfo = bannerInfo;
		DownloadSpeedUnit = downloadSpeedUnit;
		Whitelabel = whitelabel;
		IsValid = true;
	}

	public AppRemotePatcherInfo ModifyBannerInfo(AppRemotePatcherBannerInfo? newBannerInfo)
	{
		return new AppRemotePatcherInfo(newBannerInfo, DownloadSpeedUnit, Whitelabel);
	}

	public AppRemotePatcherInfo ModifyDownloadSpeedUnit(Text newDownloadSpeedUnit)
	{
		return new AppRemotePatcherInfo(BannerInfo, newDownloadSpeedUnit, Whitelabel);
	}

	public AppRemotePatcherInfo ModifyWhitelabel(bool newWhitelabel)
	{
		return new AppRemotePatcherInfo(BannerInfo, DownloadSpeedUnit, newWhitelabel);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"BannerInfo\" = {BannerInfo}, \"DownloadSpeedUnit\" = {DownloadSpeedUnit}, \"Whitelabel\" = {Whitelabel}";
		return text + "}";
	}
}
