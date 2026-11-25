using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class InvalidAppAuthKey : FunctionException
{
	public InvalidAppAuthKey(Exception inner = null)
		: base("", inner)
	{
	}
}
