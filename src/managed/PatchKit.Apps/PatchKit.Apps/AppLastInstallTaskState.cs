using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppLastInstallTaskState : IValidatedStruct
{
	public AppFilesOperationTaskProgress? Progress { get; }

	public AppInstallTaskError? Error { get; }

	public bool IsValid { get; }

	public static string Validate(AppFilesOperationTaskProgress? progress, AppInstallTaskError? error)
	{
		return null;
	}

	public AppLastInstallTaskState(AppFilesOperationTaskProgress? progress, AppInstallTaskError? error)
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

	public AppLastInstallTaskState ModifyProgress(AppFilesOperationTaskProgress? newProgress)
	{
		return new AppLastInstallTaskState(newProgress, Error);
	}

	public AppLastInstallTaskState ModifyError(AppInstallTaskError? newError)
	{
		return new AppLastInstallTaskState(Progress, newError);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Progress\" = {Progress}, \"Error\" = {Error}";
		return text + "}";
	}
}
