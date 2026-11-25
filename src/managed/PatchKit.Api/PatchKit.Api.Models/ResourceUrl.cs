using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct ResourceUrl
{
	[JsonProperty("url")]
	public string Url { get; }

	[JsonProperty("meta_url")]
	public string MetaUrl { get; }

	[JsonProperty("country")]
	public string Country { get; }

	[JsonProperty("part_size")]
	public long PartSize { get; }

	[JsonConstructor]
	public ResourceUrl(string url, string metaUrl, string country, long partSize)
	{
		this = default;
		Url = url;
		MetaUrl = metaUrl;
		Country = country;
		PartSize = partSize;
	}
}
