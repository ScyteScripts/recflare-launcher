namespace PatchKit.Core.CSharp;

public sealed class ArgCantBeNull : FunctionExecutionError
{
	public ArgCantBeNull(string name)
		: base("Argument " + name + " is null, but it can't be.", null)
	{
	}
}
