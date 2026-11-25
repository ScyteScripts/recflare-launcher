using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct StatsReport
{
	[JsonProperty("event_name")]
	public string EventName { get; }

	[JsonProperty("app_secret")]
	public string AppSecret { get; }

	[JsonProperty("app_version")]
	public int AppVersion { get; }

	[JsonProperty("sender_id")]
	public long SenderId { get; }

	[JsonProperty("caller")]
	public string Caller { get; }

	[JsonProperty("operating_system_family")]
	public string OperatingSystemFamily { get; }

	[JsonProperty("operating_system_version")]
	public string OperatingSystemVersion { get; }

	[JsonConstructor]
	public StatsReport(string eventName, string appSecret, int appVersion, long senderId, string caller, string operatingSystemFamily, string operatingSystemVersion)
	{
		this = default;
		EventName = eventName;
		AppSecret = appSecret;
		AppVersion = appVersion;
		SenderId = senderId;
		Caller = caller;
		OperatingSystemFamily = operatingSystemFamily;
		OperatingSystemVersion = operatingSystemVersion;
	}
}
