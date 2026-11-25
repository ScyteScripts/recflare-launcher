using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct AppDiffTorrentUrl
{
	[JsonProperty("url")]
	public string Url { get; }

	[JsonConstructor]
	public AppDiffTorrentUrl(string url)
	{
		this = default;
		Url = url;
	}
}
