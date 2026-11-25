namespace PatchKit.Core.CSharp;

public sealed class ValidationFailed : FunctionExecutionError
{
	public ValidationFailed(string message)
		: base(message, null)
	{
	}
}
