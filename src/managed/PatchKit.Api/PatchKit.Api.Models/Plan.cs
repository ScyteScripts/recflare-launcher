using Newtonsoft.Json;
using PatchKit.Core.CSharp;

namespace PatchKit.Api.Models;

public struct Plan
{
	[JsonProperty("capabilities")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<string>))]
	public ImmutableArray<string> Capabilities { get; }

	[JsonConstructor]
	public Plan(ImmutableArray<string> capabilities)
	{
		this = default;
		Capabilities = capabilities;
	}
}
