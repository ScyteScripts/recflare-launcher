using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class UnauthorizedAccess : FunctionException
{
	public UnauthorizedAccess(Exception inner = null)
		: base("Unauthorized access to file system region.", inner)
	{
	}
}
