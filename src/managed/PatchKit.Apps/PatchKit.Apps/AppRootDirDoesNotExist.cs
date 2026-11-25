using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class AppRootDirDoesNotExist : FunctionException
{
	public AppRootDirDoesNotExist(Exception inner = null)
		: base("App root dir doesn't exist.", inner)
	{
	}
}
