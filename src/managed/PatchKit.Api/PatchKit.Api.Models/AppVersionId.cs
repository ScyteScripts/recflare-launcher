using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct AppVersionId
{
	[JsonProperty("id")]
	public int Id { get; }

	[JsonConstructor]
	public AppVersionId(int id)
	{
		this = default;
		Id = id;
	}
}
