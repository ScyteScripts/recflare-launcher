using Newtonsoft.Json;

namespace PatchKit.Api.Models.Keys;

public struct Error
{
	[JsonProperty("message")]
	public string Message { get; }

	[JsonConstructor]
	public Error(string message)
	{
		this = default;
		Message = message;
	}
}
