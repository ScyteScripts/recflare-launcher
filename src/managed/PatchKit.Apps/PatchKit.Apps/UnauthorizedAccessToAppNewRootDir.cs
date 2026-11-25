using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class UnauthorizedAccessToAppNewRootDir : FunctionException
{
	public UnauthorizedAccessToAppNewRootDir(Exception inner = null)
		: base("Unauthorized access to app new root dir.", inner)
	{
	}
}
