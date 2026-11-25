using PatchKit.Core.CSharp;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public struct AppRemotePatcherBannerInfo : IValidatedStruct
{
	public HttpAddress Url { get; }

	public int Width { get; }

	public int Height { get; }

	public bool IsValid { get; }

	public static string Validate(HttpAddress url, int width, int height)
	{
		if (width < 0)
		{
			return "Width cannot be less or equal to 0.";
		}
		if (height < 0)
		{
			return "Height cannot be less or equal to 0.";
		}
		return null;
	}

	public AppRemotePatcherBannerInfo(HttpAddress url, int width, int height)
	{
		this = default;
		Arg.IsValid(url, "url");
		string text = Validate(url, width, height);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Url = url;
		Width = width;
		Height = height;
		IsValid = true;
	}

	public AppRemotePatcherBannerInfo ModifyUrl(HttpAddress newUrl)
	{
		return new AppRemotePatcherBannerInfo(newUrl, Width, Height);
	}

	public AppRemotePatcherBannerInfo ModifyWidth(int newWidth)
	{
		return new AppRemotePatcherBannerInfo(Url, newWidth, Height);
	}

	public AppRemotePatcherBannerInfo ModifyHeight(int newHeight)
	{
		return new AppRemotePatcherBannerInfo(Url, Width, newHeight);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Url\" = {Url}, \"Width\" = {Width}, \"Height\" = {Height}";
		return text + "}";
	}
}
