using Newtonsoft.Json;

namespace PatchKit.Api.Models.Keys;

public struct LicenseKeysCollectionMetaData
{
	[JsonProperty("total_count")]
	public int TotalCount { get; }

	[JsonConstructor]
	public LicenseKeysCollectionMetaData(int totalCount)
	{
		this = default;
		TotalCount = totalCount;
	}
}
