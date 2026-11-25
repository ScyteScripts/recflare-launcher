using Newtonsoft.Json;
using PatchKit.Core.CSharp;

namespace PatchKit.Api.Models;

public struct Chunks
{
	[JsonProperty("size")]
	public int Size { get; }

	[JsonProperty("hashes")]
	[JsonConverter(typeof(ImmutableArrayJsonConverter<string>))]
	public ImmutableArray<string> Hashes { get; }

	[JsonConstructor]
	public Chunks(int size, ImmutableArray<string> hashes)
	{
		this = default;
		Size = size;
		Hashes = hashes;
	}
}
