using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class AppNewRootDirIsNotEmpty : FunctionException
{
	public AppNewRootDirIsNotEmpty(Exception inner = null)
		: base("App new root dir is not empty.", inner)
	{
	}
}
