using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class OutOfAppRootDirFreeSpace : FunctionException
{
	public OutOfAppRootDirFreeSpace(Exception inner = null)
		: base("Out of app new root dir free space.", inner)
	{
	}
}
