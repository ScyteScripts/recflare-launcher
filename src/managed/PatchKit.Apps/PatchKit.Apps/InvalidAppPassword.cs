using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class InvalidAppPassword : FunctionException
{
	public InvalidAppPassword(Exception inner = null)
		: base("", inner)
	{
	}
}
