using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct App
{
	[JsonProperty("id")]
	public int Id { get; }

	[JsonProperty("secret")]
	public string Secret { get; }

	[JsonProperty("platform")]
	public string Platform { get; }

	[JsonProperty("name")]
	public string Name { get; }

	[JsonProperty("display_name")]
	public string DisplayName { get; }

	[JsonProperty("author")]
	public string Author { get; }

	[JsonProperty("use_keys")]
	public bool UseKeys { get; }

	[JsonProperty("use_token")]
	public bool UseToken { get; }

	[JsonProperty("use_password")]
	public bool UsePassword { get; }

	[JsonProperty("publish_method")]
	public string PublishMethod { get; }

	[JsonProperty("current_version")]
	public int CurrentVersion { get; }

	[JsonProperty("lowest_version_with_diff")]
	public int LowestVersionWithDiff { get; }

	[JsonProperty("patcher_banner_image")]
	public string PatcherBannerImage { get; }

	[JsonProperty("patcher_banner_image_dimensions")]
	public PatcherBannerImageDimensions PatcherBannerImageDimensions { get; }

	[JsonProperty("patcher_banner_image_updated_at")]
	public string PatcherBannerImageUpdatedAt { get; }

	[JsonProperty("patcher_download_speed_unit")]
	public string PatcherDownloadSpeedUnit { get; }

	[JsonProperty("patcher_whitelabel")]
	public bool PatcherWhitelabel { get; }

	[JsonProperty("patcher_secret")]
	public string PatcherSecret { get; }

	[JsonConstructor]
	public App(int id, string secret, string platform, string name, string displayName, string author, bool useKeys, bool useToken, bool usePassword, string publishMethod, int currentVersion, int lowestVersionWithDiff, string patcherBannerImage, PatcherBannerImageDimensions patcherBannerImageDimensions, string patcherBannerImageUpdatedAt, string patcherDownloadSpeedUnit, bool patcherWhitelabel, string patcherSecret)
	{
		this = default;
		Id = id;
		Secret = secret;
		Platform = platform;
		Name = name;
		DisplayName = displayName;
		Author = author;
		UseKeys = useKeys;
		UseToken = useToken;
		UsePassword = usePassword;
		PublishMethod = publishMethod;
		CurrentVersion = currentVersion;
		LowestVersionWithDiff = lowestVersionWithDiff;
		PatcherBannerImage = patcherBannerImage;
		PatcherBannerImageDimensions = patcherBannerImageDimensions;
		PatcherBannerImageUpdatedAt = patcherBannerImageUpdatedAt;
		PatcherDownloadSpeedUnit = patcherDownloadSpeedUnit;
		PatcherWhitelabel = patcherWhitelabel;
		PatcherSecret = patcherSecret;
	}
}
