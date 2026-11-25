using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct Changelog
{
	[JsonProperty("version_id")]
	public int VersionId { get; }

	[JsonProperty("version_label")]
	public string VersionLabel { get; }

	[JsonProperty("changes")]
	public string Changes { get; }

	[JsonConstructor]
	public Changelog(int versionId, string versionLabel, string changes)
	{
		this = default;
		VersionId = versionId;
		VersionLabel = versionLabel;
		Changes = changes;
	}
}
