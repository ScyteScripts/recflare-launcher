using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct Error
{
	[JsonProperty("message")]
	public string Message { get; }

	[JsonProperty("symbol")]
	public string Symbol { get; }

	[JsonConstructor]
	public Error(string message, string symbol)
	{
		this = default;
		Message = message;
		Symbol = symbol;
	}
}
