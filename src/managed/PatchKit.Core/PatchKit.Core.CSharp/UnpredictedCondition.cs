using System;

namespace PatchKit.Core.CSharp;

public sealed class UnpredictedCondition : FunctionExecutionError
{
	public UnpredictedCondition(string message, Exception inner = null)
		: base(message, inner)
	{
	}
}
