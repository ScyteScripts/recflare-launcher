using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public struct UpdateAppProgress : IValidatedStruct
{
	public BytesCount? PackagesCurrentFileProcessedBytesCount { get; }

	public BytesCount PackagesProcessedBytesCount { get; }

	public BytesCount PackagesTotalBytesCount { get; }

	public BytesCount PackagesProcessedBytesCountPerSecond { get; }

	public BytesCount FilesProcessedBytesCount { get; }

	public BytesCount FilesTotalBytesCount { get; }

	public BytesCount FilesProcessedBytesCountPerSecond { get; }

	public bool IsValid { get; }

	public static string Validate(BytesCount? packagesCurrentFileProcessedBytesCount, BytesCount packagesProcessedBytesCount, BytesCount packagesTotalBytesCount, BytesCount packagesProcessedBytesCountPerSecond, BytesCount filesProcessedBytesCount, BytesCount filesTotalBytesCount, BytesCount filesProcessedBytesCountPerSecond)
	{
		return null;
	}

	public UpdateAppProgress(BytesCount? packagesCurrentFileProcessedBytesCount, BytesCount packagesProcessedBytesCount, BytesCount packagesTotalBytesCount, BytesCount packagesProcessedBytesCountPerSecond, BytesCount filesProcessedBytesCount, BytesCount filesTotalBytesCount, BytesCount filesProcessedBytesCountPerSecond)
	{
		this = default;
		Arg.IsValid(packagesCurrentFileProcessedBytesCount, "packagesCurrentFileProcessedBytesCount");
		Arg.IsValid(packagesProcessedBytesCount, "packagesProcessedBytesCount");
		Arg.IsValid(packagesTotalBytesCount, "packagesTotalBytesCount");
		Arg.IsValid(packagesProcessedBytesCountPerSecond, "packagesProcessedBytesCountPerSecond");
		Arg.IsValid(filesProcessedBytesCount, "filesProcessedBytesCount");
		Arg.IsValid(filesTotalBytesCount, "filesTotalBytesCount");
		Arg.IsValid(filesProcessedBytesCountPerSecond, "filesProcessedBytesCountPerSecond");
		string text = Validate(packagesCurrentFileProcessedBytesCount, packagesProcessedBytesCount, packagesTotalBytesCount, packagesProcessedBytesCountPerSecond, filesProcessedBytesCount, filesTotalBytesCount, filesProcessedBytesCountPerSecond);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		PackagesCurrentFileProcessedBytesCount = packagesCurrentFileProcessedBytesCount;
		PackagesProcessedBytesCount = packagesProcessedBytesCount;
		PackagesTotalBytesCount = packagesTotalBytesCount;
		PackagesProcessedBytesCountPerSecond = packagesProcessedBytesCountPerSecond;
		FilesProcessedBytesCount = filesProcessedBytesCount;
		FilesTotalBytesCount = filesTotalBytesCount;
		FilesProcessedBytesCountPerSecond = filesProcessedBytesCountPerSecond;
		IsValid = true;
	}

	public UpdateAppProgress ModifyPackagesCurrentFileProcessedBytesCount(BytesCount? newPackagesCurrentFileProcessedBytesCount)
	{
		return new UpdateAppProgress(newPackagesCurrentFileProcessedBytesCount, PackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public UpdateAppProgress ModifyPackagesProcessedBytesCount(BytesCount newPackagesProcessedBytesCount)
	{
		return new UpdateAppProgress(PackagesCurrentFileProcessedBytesCount, newPackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public UpdateAppProgress ModifyPackagesTotalBytesCount(BytesCount newPackagesTotalBytesCount)
	{
		return new UpdateAppProgress(PackagesCurrentFileProcessedBytesCount, PackagesProcessedBytesCount, newPackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public UpdateAppProgress ModifyPackagesProcessedBytesCountPerSecond(BytesCount newPackagesProcessedBytesCountPerSecond)
	{
		return new UpdateAppProgress(PackagesCurrentFileProcessedBytesCount, PackagesProcessedBytesCount, PackagesTotalBytesCount, newPackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public UpdateAppProgress ModifyFilesProcessedBytesCount(BytesCount newFilesProcessedBytesCount)
	{
		return new UpdateAppProgress(PackagesCurrentFileProcessedBytesCount, PackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, newFilesProcessedBytesCount, FilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public UpdateAppProgress ModifyFilesTotalBytesCount(BytesCount newFilesTotalBytesCount)
	{
		return new UpdateAppProgress(PackagesCurrentFileProcessedBytesCount, PackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, newFilesTotalBytesCount, FilesProcessedBytesCountPerSecond);
	}

	public UpdateAppProgress ModifyFilesProcessedBytesCountPerSecond(BytesCount newFilesProcessedBytesCountPerSecond)
	{
		return new UpdateAppProgress(PackagesCurrentFileProcessedBytesCount, PackagesProcessedBytesCount, PackagesTotalBytesCount, PackagesProcessedBytesCountPerSecond, FilesProcessedBytesCount, FilesTotalBytesCount, newFilesProcessedBytesCountPerSecond);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"PackagesCurrentFileProcessedBytesCount\" = {PackagesCurrentFileProcessedBytesCount}, \"PackagesProcessedBytesCount\" = {PackagesProcessedBytesCount}, \"PackagesTotalBytesCount\" = {PackagesTotalBytesCount}, \"PackagesProcessedBytesCountPerSecond\" = {PackagesProcessedBytesCountPerSecond}, \"FilesProcessedBytesCount\" = {FilesProcessedBytesCount}, \"FilesTotalBytesCount\" = {FilesTotalBytesCount}, \"FilesProcessedBytesCountPerSecond\" = {FilesProcessedBytesCountPerSecond}";
		return text + "}";
	}
}
