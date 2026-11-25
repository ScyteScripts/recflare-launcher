using System;

namespace PatchKit.Core.CSharp;

public sealed class FunctionFatalError : Exception
{
	public FunctionFatalError(Exception inner)
		: base("Function fatal error.", inner)
	{
	}
}
