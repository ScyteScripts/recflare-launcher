using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct AppVersionPatchLink
{
	[JsonProperty("id")]
	public int Id { get; }

	[JsonProperty("status")]
	public string Status { get; }

	[JsonProperty("source_version")]
	public int SourceVersion { get; }

	[JsonProperty("target_version")]
	public int TargetVersion { get; }

	[JsonConstructor]
	public AppVersionPatchLink(int id, string status, int sourceVersion, int targetVersion)
	{
		this = default;
		Id = id;
		Status = status;
		SourceVersion = sourceVersion;
		TargetVersion = targetVersion;
	}
}
