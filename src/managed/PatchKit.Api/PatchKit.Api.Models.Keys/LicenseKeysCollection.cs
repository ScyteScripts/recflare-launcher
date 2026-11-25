using Newtonsoft.Json;
using PatchKit.Core.CSharp;

namespace PatchKit.Api.Models.Keys;

public struct LicenseKeysCollection
{
	[JsonProperty("_metadata")]
	public LicenseKeysCollectionMetaData Metadata { get; }

	[JsonProperty("keys")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<LicenseKey>))]
	public ImmutableArray<LicenseKey> Keys { get; }

	[JsonConstructor]
	public LicenseKeysCollection(LicenseKeysCollectionMetaData metadata, ImmutableArray<LicenseKey> keys)
	{
		this = default;
		Metadata = metadata;
		Keys = keys;
	}
}
