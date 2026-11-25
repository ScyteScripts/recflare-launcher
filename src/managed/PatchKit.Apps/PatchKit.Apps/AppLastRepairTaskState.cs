using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppLastRepairTaskState : IValidatedStruct
{
	public AppFilesOperationTaskProgress? Progress { get; }

	public AppRepairTaskError? Error { get; }

	public bool IsValid { get; }

	public static string Validate(AppFilesOperationTaskProgress? progress, AppRepairTaskError? error)
	{
		return null;
	}

	public AppLastRepairTaskState(AppFilesOperationTaskProgress? progress, AppRepairTaskError? error)
	{
		this = default;
		Arg.IsValid(progress, "progress");
		string text = Validate(progress, error);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Progress = progress;
		Error = error;
		IsValid = true;
	}

	public AppLastRepairTaskState ModifyProgress(AppFilesOperationTaskProgress? newProgress)
	{
		return new AppLastRepairTaskState(newProgress, Error);
	}

	public AppLastRepairTaskState ModifyError(AppRepairTaskError? newError)
	{
		return new AppLastRepairTaskState(Progress, newError);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Progress\" = {Progress}, \"Error\" = {Error}";
		return text + "}";
	}
}
