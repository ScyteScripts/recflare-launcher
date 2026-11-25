using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class AppIsNotInstalled : FunctionException
{
	public AppIsNotInstalled(Exception inner = null)
		: base("", inner)
	{
	}
}
