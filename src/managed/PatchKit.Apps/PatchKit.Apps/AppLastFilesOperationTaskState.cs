using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppLastFilesOperationTaskState : IValidatedStruct
{
	public AppLastInstallTaskState? AppLastInstallTaskState { get; }

	public AppLastUpdateTaskState? AppLastUpdateTaskState { get; }

	public AppLastRepairTaskState? AppLastRepairTaskState { get; }

	public bool IsValid { get; }

	public static string Validate(AppLastInstallTaskState? appLastInstallTaskState, AppLastUpdateTaskState? appLastUpdateTaskState, AppLastRepairTaskState? appLastRepairTaskState)
	{
		return null;
	}

	public AppLastFilesOperationTaskState(AppLastInstallTaskState? appLastInstallTaskState, AppLastUpdateTaskState? appLastUpdateTaskState, AppLastRepairTaskState? appLastRepairTaskState)
	{
		this = default;
		Arg.IsValid(appLastInstallTaskState, "appLastInstallTaskState");
		Arg.IsValid(appLastUpdateTaskState, "appLastUpdateTaskState");
		Arg.IsValid(appLastRepairTaskState, "appLastRepairTaskState");
		string text = Validate(appLastInstallTaskState, appLastUpdateTaskState, appLastRepairTaskState);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		AppLastInstallTaskState = appLastInstallTaskState;
		AppLastUpdateTaskState = appLastUpdateTaskState;
		AppLastRepairTaskState = appLastRepairTaskState;
		IsValid = true;
	}

	public AppLastFilesOperationTaskState ModifyAppLastInstallTaskState(AppLastInstallTaskState? newAppLastInstallTaskState)
	{
		return new AppLastFilesOperationTaskState(newAppLastInstallTaskState, AppLastUpdateTaskState, AppLastRepairTaskState);
	}

	public AppLastFilesOperationTaskState ModifyAppLastUpdateTaskState(AppLastUpdateTaskState? newAppLastUpdateTaskState)
	{
		return new AppLastFilesOperationTaskState(AppLastInstallTaskState, newAppLastUpdateTaskState, AppLastRepairTaskState);
	}

	public AppLastFilesOperationTaskState ModifyAppLastRepairTaskState(AppLastRepairTaskState? newAppLastRepairTaskState)
	{
		return new AppLastFilesOperationTaskState(AppLastInstallTaskState, AppLastUpdateTaskState, newAppLastRepairTaskState);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"AppLastInstallTaskState\" = {AppLastInstallTaskState}, \"AppLastUpdateTaskState\" = {AppLastUpdateTaskState}, \"AppLastRepairTaskState\" = {AppLastRepairTaskState}";
		return text + "}";
	}
}
