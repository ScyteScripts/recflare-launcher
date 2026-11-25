using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemoteVersionEntryInfo : IValidatedStruct
{
	public AppPath Path
	{
		get
		{
			if (FileInfo.HasValue)
			{
				return FileInfo.Value.Path;
			}
			if (DirInfo.HasValue)
			{
				return DirInfo.Value.Path;
			}
			throw new UnpredictedCondition("Invalid values");
		}
	}

	public AppRemoteVersionFileInfo? FileInfo { get; }

	public AppRemoteVersionDirInfo? DirInfo { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemoteVersionFileInfo? fileInfo, AppRemoteVersionDirInfo? dirInfo)
	{
		if (!fileInfo.HasValue && !dirInfo.HasValue)
		{
			return "At least one value must be provided.";
		}
		if (fileInfo.HasValue && dirInfo.HasValue)
		{
			return "Only one value must be provided.";
		}
		return null;
	}

	public static implicit operator AppRemoteVersionEntryInfo(AppRemoteVersionFileInfo value)
	{
		return new AppRemoteVersionEntryInfo(value, null);
	}

	public static implicit operator AppRemoteVersionEntryInfo(AppRemoteVersionDirInfo value)
	{
		return new AppRemoteVersionEntryInfo(null, value);
	}

	public AppRemoteVersionEntryInfo(AppRemoteVersionFileInfo? fileInfo, AppRemoteVersionDirInfo? dirInfo)
	{
		this = default;
		Arg.IsValid(fileInfo, "fileInfo");
		Arg.IsValid(dirInfo, "dirInfo");
		string text = Validate(fileInfo, dirInfo);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		FileInfo = fileInfo;
		DirInfo = dirInfo;
		IsValid = true;
	}

	public AppRemoteVersionEntryInfo ModifyFileInfo(AppRemoteVersionFileInfo? newFileInfo)
	{
		return new AppRemoteVersionEntryInfo(newFileInfo, DirInfo);
	}

	public AppRemoteVersionEntryInfo ModifyDirInfo(AppRemoteVersionDirInfo? newDirInfo)
	{
		return new AppRemoteVersionEntryInfo(FileInfo, newDirInfo);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"FileInfo\" = {FileInfo}, \"DirInfo\" = {DirInfo}";
		return text + "}";
	}
}
