using System;

namespace PatchKit.Core.CSharp;

public abstract class FunctionExecutionError : Exception
{
	public FunctionExecutionError(string message, Exception inner)
		: base(message, inner)
	{
	}
}
