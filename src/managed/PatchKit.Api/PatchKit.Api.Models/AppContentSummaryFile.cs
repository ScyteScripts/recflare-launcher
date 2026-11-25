using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct AppContentSummaryFile
{
	[JsonProperty("path")]
	public string Path { get; }

	[JsonProperty("hash")]
	public string Hash { get; }

	[JsonProperty("size")]
	public long Size { get; }

	[JsonProperty("flags")]
	public string Flags { get; }

	[JsonConstructor]
	public AppContentSummaryFile(string path, string hash, long size, string flags)
	{
		this = default;
		Path = path;
		Hash = hash;
		Size = size;
		Flags = flags;
	}
}
