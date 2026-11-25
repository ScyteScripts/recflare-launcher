using System.Threading;

namespace PatchKit.Apps;

public class AppRepairTaskState
{
	public int Id;

	public AppFilesOperationTaskProgress? Progress;

	public bool HasFinished;

	public AppRepairTaskError? Error;

	public CancellationTokenSource CancellationTokenSource;
}
