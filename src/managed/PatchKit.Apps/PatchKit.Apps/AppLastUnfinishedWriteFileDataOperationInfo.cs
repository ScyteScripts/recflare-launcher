using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public struct AppLastUnfinishedWriteFileDataOperationInfo : IValidatedStruct
{
	public AppPath FilePath { get; }

	public AppVersionId FileVersionId { get; }

	public BytesCount FileLastBytesCount { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath filePath, AppVersionId fileVersionId, BytesCount fileLastBytesCount)
	{
		return null;
	}

	public AppLastUnfinishedWriteFileDataOperationInfo(AppPath filePath, AppVersionId fileVersionId, BytesCount fileLastBytesCount)
	{
		this = default;
		Arg.IsValid(filePath, "filePath");
		Arg.IsValid(fileVersionId, "fileVersionId");
		Arg.IsValid(fileLastBytesCount, "fileLastBytesCount");
		string text = Validate(filePath, fileVersionId, fileLastBytesCount);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		FilePath = filePath;
		FileVersionId = fileVersionId;
		FileLastBytesCount = fileLastBytesCount;
		IsValid = true;
	}

	public AppLastUnfinishedWriteFileDataOperationInfo ModifyFilePath(AppPath newFilePath)
	{
		return new AppLastUnfinishedWriteFileDataOperationInfo(newFilePath, FileVersionId, FileLastBytesCount);
	}

	public AppLastUnfinishedWriteFileDataOperationInfo ModifyFileVersionId(AppVersionId newFileVersionId)
	{
		return new AppLastUnfinishedWriteFileDataOperationInfo(FilePath, newFileVersionId, FileLastBytesCount);
	}

	public AppLastUnfinishedWriteFileDataOperationInfo ModifyFileLastBytesCount(BytesCount newFileLastBytesCount)
	{
		return new AppLastUnfinishedWriteFileDataOperationInfo(FilePath, FileVersionId, newFileLastBytesCount);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"FilePath\" = {FilePath}, \"FileVersionId\" = {FileVersionId}, \"FileLastBytesCount\" = {FileLastBytesCount}";
		return text + "}";
	}
}
