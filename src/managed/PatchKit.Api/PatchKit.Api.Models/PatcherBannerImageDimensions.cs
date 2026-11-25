using Newtonsoft.Json;

namespace PatchKit.Api.Models;

public struct PatcherBannerImageDimensions
{
	[JsonProperty("x")]
	public int X { get; }

	[JsonProperty("y")]
	public int Y { get; }

	[JsonConstructor]
	public PatcherBannerImageDimensions(int x, int y)
	{
		this = default;
		X = x;
		Y = y;
	}
}
