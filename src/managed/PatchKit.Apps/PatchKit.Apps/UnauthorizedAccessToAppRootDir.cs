using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class UnauthorizedAccessToAppRootDir : FunctionException
{
	public UnauthorizedAccessToAppRootDir(Exception inner = null)
		: base("Unauthorized access to app root dir.", inner)
	{
	}
}
