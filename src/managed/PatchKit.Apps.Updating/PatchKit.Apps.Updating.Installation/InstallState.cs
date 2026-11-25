using System.Collections.Generic;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public class InstallState
{
	public BytesCount FilesProcessedBytesCount = new BytesCount(0L);

	public InstallPackageDataFileTask CurrentInstallPackageDataFileTask;

	public InstallPackagePatchFileTask CurrentInstallPackagePatchFileTask;

	public Queue<InstallPackagePatchFileTask> InstallPackagePatchFileTasksQueue { get; } = new Queue<InstallPackagePatchFileTask>();
}
