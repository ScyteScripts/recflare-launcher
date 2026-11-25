using System;

namespace PatchKit.Core.CSharp;

public abstract class FunctionException : Exception
{
	public FunctionException(string message, Exception inner = null)
		: base(message, inner)
	{
	}
}
