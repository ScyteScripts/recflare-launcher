using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct Job
{
	[JsonProperty("job_guid")]
	public string JobGuid { get; }

	[JsonConstructor]
	public Job(string jobGuid)
	{
		this = default;
		JobGuid = jobGuid;
	}
}
