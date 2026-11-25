using System.Collections.Generic;

namespace PatchKit.Apps.Updating;

public class AppTasksState
{
	public int LastAppTaskId;

	public readonly List<AppInstallTaskState> AppInstallTasks = new List<AppInstallTaskState>();

	public readonly List<AppUpdateTaskState> AppUpdateTasks = new List<AppUpdateTaskState>();

	public readonly List<AppRepairTaskState> AppRepairTasks = new List<AppRepairTaskState>();

	public readonly List<AppMoveTaskState> AppMoveTasks = new List<AppMoveTaskState>();

	public readonly List<AppUninstallTaskState> AppUninstallTasks = new List<AppUninstallTaskState>();
}
