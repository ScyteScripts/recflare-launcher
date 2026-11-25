using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class AppNeedsRepair : FunctionException
{
	public AppNeedsRepair(Exception inner = null)
		: base("App needs repair.", inner)
	{
	}
}
