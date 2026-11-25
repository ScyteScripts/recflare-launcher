using System;

namespace PatchKit.Core.CSharp;

public sealed class UnpredictedException : FunctionExecutionError
{
	public UnpredictedException(Exception other)
		: base("Unpredicted exception occured.", other)
	{
	}
}
