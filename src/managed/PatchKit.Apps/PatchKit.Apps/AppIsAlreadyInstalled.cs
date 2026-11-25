using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class AppIsAlreadyInstalled : FunctionException
{
	public AppIsAlreadyInstalled(Exception inner = null)
		: base("App is already installed.", inner)
	{
	}
}
