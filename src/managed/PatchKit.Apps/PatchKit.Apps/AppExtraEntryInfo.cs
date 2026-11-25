using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public struct AppExtraEntryInfo : IValidatedStruct
{
	public Path Path { get; }

	public bool IsValid { get; }

	public static string Validate(Path path)
	{
		return null;
	}

	public AppExtraEntryInfo(Path path)
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

	public AppExtraEntryInfo ModifyPath(Path newPath)
	{
		return new AppExtraEntryInfo(newPath);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Path\" = {Path}";
		return text + "}";
	}
}
