using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct AppAuthentication
{
	[JsonProperty("jwt")]
	public string Jwt { get; }

	[JsonConstructor]
	public AppAuthentication(string jwt)
	{
		this = default;
		Jwt = jwt;
	}
}
