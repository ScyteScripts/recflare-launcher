using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class ExpiredAppAuthKey : FunctionException
{
	public ExpiredAppAuthKey(Exception inner = null)
		: base("", inner)
	{
	}
}
