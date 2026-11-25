namespace PatchKit.Core.CSharp;

public sealed class UsageOfDisposedObject : FunctionExecutionError
{
	public UsageOfDisposedObject()
		: base("Cannot perform any operations on disposed object.", null)
	{
	}
}
