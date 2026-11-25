using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppLastUpdateTaskState : IValidatedStruct
{
	public AppFilesOperationTaskProgress? Progress { get; }

	public AppUpdateTaskError? Error { get; }

	public bool IsValid { get; }

	public static string Validate(AppFilesOperationTaskProgress? progress, AppUpdateTaskError? error)
	{
		return null;
	}

	public AppLastUpdateTaskState(AppFilesOperationTaskProgress? progress, AppUpdateTaskError? error)
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

	public AppLastUpdateTaskState ModifyProgress(AppFilesOperationTaskProgress? newProgress)
	{
		return new AppLastUpdateTaskState(newProgress, Error);
	}

	public AppLastUpdateTaskState ModifyError(AppUpdateTaskError? newError)
	{
		return new AppLastUpdateTaskState(Progress, newError);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Progress\" = {Progress}, \"Error\" = {Error}";
		return text + "}";
	}
}
