using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class OutOfAppNewRootDirFreeSpace : FunctionException
{
	public OutOfAppNewRootDirFreeSpace(Exception inner = null)
		: base("Out of app new root dir free space.", inner)
	{
	}
}
