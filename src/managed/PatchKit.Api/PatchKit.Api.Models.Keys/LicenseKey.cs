using Newtonsoft.Json;

namespace PatchKit.Api.Models.Keys;

public struct LicenseKey
{
	[JsonProperty("key")]
	public string Key { get; }

	[JsonProperty("app_secret")]
	public string AppSecret { get; }

	[JsonProperty("key_secret")]
	public string KeySecret { get; }

	[JsonProperty("collection_id")]
	public int CollectionId { get; }

	[JsonProperty("registrations")]
	public int Registrations { get; }

	[JsonProperty("blocked")]
	public bool Blocked { get; }

	[JsonConstructor]
	public LicenseKey(string key, string appSecret, string keySecret, int collectionId, int registrations, bool blocked)
	{
		this = default;
		Key = key;
		AppSecret = appSecret;
		KeySecret = keySecret;
		CollectionId = collectionId;
		Registrations = registrations;
		Blocked = blocked;
	}
}
