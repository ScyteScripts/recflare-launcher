using Newtonsoft.Json;

namespace PatchKit.Api.Models.Keys;

public struct Job
{
	[JsonProperty("guid")]
	public string Guid { get; }

	[JsonProperty("pending")]
	public bool Pending { get; }

	[JsonProperty("finished")]
	public bool Finished { get; }

	[JsonConstructor]
	public Job(string guid, bool pending, bool finished)
	{
		this = default;
		Guid = guid;
		Pending = pending;
		Finished = finished;
	}
}
