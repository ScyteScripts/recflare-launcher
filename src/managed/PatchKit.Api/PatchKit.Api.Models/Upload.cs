using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct Upload
{
	[JsonProperty("id")]
	public int Id { get; }

	[JsonConstructor]
	public Upload(int id)
	{
		this = default;
		Id = id;
	}
}
