using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class OutOfFreeDiskSpace : FunctionException
{
	public OutOfFreeDiskSpace(Exception inner = null)
		: base("", inner)
	{
	}
}
