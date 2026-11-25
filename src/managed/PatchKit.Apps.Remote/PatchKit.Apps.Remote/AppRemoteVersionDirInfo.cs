using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemoteVersionDirInfo : IValidatedStruct
{
	public AppPath Path { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath path)
	{
		return null;
	}

	public AppRemoteVersionDirInfo(AppPath path)
	{
		this = default;
		Arg.IsValid(path, "path");
		string text = Validate(path);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Path = path;
		IsValid = true;
	}

	public AppRemoteVersionDirInfo ModifyPath(AppPath newPath)
	{
		return new AppRemoteVersionDirInfo(newPath);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}";
		return text + "}";
	}
}
