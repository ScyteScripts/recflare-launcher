using System;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class InstallPackagePatchFileTask
{
	public BytesCount FileProcessedBytesCount = new BytesCount(0L);

	public bool IsPatchingFinished = false;

	public Exception PatchingException = null;

	public AppPath FilePath { get; }

	public AppVersionPatchId VersionPatchId { get; }

	public Path PatchFilePath { get; }

	public Path PatchedFilePath { get; }

	public ITempDirDisposable TempDir { get; }

	public InstallPackagePatchFileTask(AppPath filePath, AppVersionPatchId versionPatchId, Path patchFilePath, Path patchedFilePath, ITempDirDisposable tempDir)
	{
		FilePath = filePath;
		VersionPatchId = versionPatchId;
		PatchFilePath = patchFilePath;
		PatchedFilePath = patchedFilePath;
		TempDir = tempDir;
	}
}
