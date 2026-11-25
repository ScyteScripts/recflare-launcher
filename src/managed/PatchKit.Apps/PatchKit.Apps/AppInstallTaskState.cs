using System.Threading;

namespace PatchKit.Apps;

public class AppInstallTaskState
{
	public int Id;

	public AppFilesOperationTaskProgress? Progress;

	public bool HasFinished;

	public AppInstallTaskError? Error;

	public CancellationTokenSource CancellationTokenSource;
}
