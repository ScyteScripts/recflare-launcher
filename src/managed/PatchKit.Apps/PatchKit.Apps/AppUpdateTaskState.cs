using System.Threading;

namespace PatchKit.Apps;

public class AppUpdateTaskState
{
	public int Id;

	public AppFilesOperationTaskProgress? Progress;

	public bool HasFinished;

	public AppUpdateTaskError? Error;

	public CancellationTokenSource CancellationTokenSource;
}
