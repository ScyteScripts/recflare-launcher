namespace PatchKit.Core.CSharp;

public sealed class ArgNotValid : FunctionExecutionError
{
	public ArgNotValid(string name)
		: base("Argument " + name + " is not valid.", null)
	{
	}
}
