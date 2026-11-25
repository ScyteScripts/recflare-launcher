using Newtonsoft.Json;
using PatchKit.Core.CSharp;

namespace PatchKit.Api.Models;

public struct AppVersionPatchLinks
{
	[JsonProperty("incoming")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<AppVersionPatchLink>))]
	public ImmutableArray<AppVersionPatchLink> Incoming { get; }

	[JsonProperty("outgoing")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<AppVersionPatchLink>))]
	public ImmutableArray<AppVersionPatchLink> Outgoing { get; }

	[JsonConstructor]
	public AppVersionPatchLinks(ImmutableArray<AppVersionPatchLink> incoming, ImmutableArray<AppVersionPatchLink> outgoing)
	{
		this = default;
		Incoming = incoming;
		Outgoing = outgoing;
	}
}
