using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public struct AppFilesOperationTaskProgress : IValidatedStruct
{
	public BytesCount PackagesProcessedBytesCount { get; }

	public BytesCount PackagesTotalBytesCount { get; }

	public BytesCount PackagesProcessedBytesCountPerSecond { get; }

	public BytesCount FilesProcessedBytesCount { get; }

	public BytesCount FilesTotalBytesCount { get; }

	public BytesCount FilesProcessedBytesCountPerSecond { get; }

	public bool IsValid { get; }

	public static string Validate(BytesCount packagesProcessedBytesCount, BytesCount packagesTotalBytesCount, BytesCount packagesProcessedBytesCountPerSecond, BytesCount filesProcessedBytesCount, BytesCount filesTotalBytesCount, BytesCount filesProcessedBytesCountPerSecond)
	{
		return null;
	}

	public AppFilesOperationTaskProgress(BytesCount packagesProcessedBytesCount, BytesCount packagesTotalBytesCount, BytesCount packagesProcessedBytesCountPerSecond, BytesCount filesProcessedBytesCount, BytesCount filesTotalBytesCount, BytesCount filesProcessedBytesCountPerSecond)
	{
		this = default;
		Arg.IsValid(packagesProcessedBytesCount, "packagesProcessedBytesCount");
		Arg.IsValid(packagesTotalBytesCount, "packagesTotalBytesCount");
		Arg.IsValid(packagesProcessedBytesCountPerSecond, "packagesProcessedBytesCountPerSecond");
		Arg.IsValid(filesProcessedBytesCount, "filesProcessedBytesCount");
		Arg.IsValid(filesTotalBytesCount, "filesTotalBytesCount");
		Arg.IsValid(filesProcessedBytesCountPerSecond, "filesProcessedBytesCountPerSecond");
		string text = Validate(packagesProcessedBytesCount, packagesTotalBytesCount, packagesProcessedBytesCountPerSecond, filesProcessedBytesCount, filesTotalBytesCount, filesProcessedBytesCountPerSecond);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		PackagesProcessedBytesCount = packagesProcessedBytesCount;
		PackagesTotalBytesCount = packagesTotalBytesCount;
		PackagesProcessedBytesCountPerSecond = packagesProcessedBytesCountPerSecond;
		FilesProcessedBytesCount = filesProcessedBytesCount;
		FilesTotalBytesCount = filesTotalBytesCount;
		FilesProcessedBytesCountPerSecond = filesProcessedBytesCountPerSecond;
		IsValid = true;
	}

	public AppFilesOperationTaskProgress ModifyPackagesProcessedBytesCount(BytesCount newPackagesProcessedBytesCount)
	{
		return new AppFilesOperationTaskProgress(newPackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public AppFilesOperationTaskProgress ModifyPackagesTotalBytesCount(BytesCount newPackagesTotalBytesCount)
	{
		return new AppFilesOperationTaskProgress(PackagesProcessedBytesCount, newPackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public AppFilesOperationTaskProgress ModifyPackagesProcessedBytesCountPerSecond(BytesCount newPackagesProcessedBytesCountPerSecond)
	{
		return new AppFilesOperationTaskProgress(PackagesProcessedBytesCount, PackagesTotalBytesCount, newPackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public AppFilesOperationTaskProgress ModifyFilesProcessedBytesCount(BytesCount newFilesProcessedBytesCount)
	{
		return new AppFilesOperationTaskProgress(PackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, newFilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public AppFilesOperationTaskProgress ModifyFilesTotalBytesCount(BytesCount newFilesTotalBytesCount)
	{
		return new AppFilesOperationTaskProgress(PackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, newFilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public AppFilesOperationTaskProgress ModifyFilesProcessedBytesCountPerSecond(BytesCount newFilesProcessedBytesCountPerSecond)
	{
		return new AppFilesOperationTaskProgress(PackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, newFilesProcessedBytesCountPerSecond);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"PackagesProcessedBytesCount\" = {PackagesProcessedBytesCount}, \"PackagesTotalBytesCount\" = {PackagesTotalBytesCount}, \"PackagesProcessedBytesCountPerSecond\" = {PackagesProcessedBytesCountPerSecond}, \"FilesProcessedBytesCount\" = {FilesProcessedBytesCount}, \"FilesTotalBytesCount\" = {FilesTotalBytesCount}, \"FilesProcessedBytesCountPerSecond\" = {FilesProcessedBytesCountPerSecond}";
		return text + "}";
	}
}
