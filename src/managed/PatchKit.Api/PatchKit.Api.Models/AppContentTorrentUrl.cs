using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct AppContentTorrentUrl
{
	[JsonProperty("url")]
	public string Url { get; }

	[JsonConstructor]
	public AppContentTorrentUrl(string url)
	{
		this = default;
		Url = url;
	}
}
