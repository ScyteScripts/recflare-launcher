using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class StartingAppTaskIsForbidden : FunctionException
{
	public StartingAppTaskIsForbidden(Exception inner = null)
		: base("", inner)
	{
	}
}
