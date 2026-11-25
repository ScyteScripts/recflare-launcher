using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class AppRootDirIsNotEmpty : FunctionException
{
	public AppRootDirIsNotEmpty(Exception inner = null)
		: base("App root dir is not empty.", inner)
	{
	}
}
