using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class OtherAppRootDirIsConflictingWithAppRootDir : FunctionException
{
	public OtherAppRootDirIsConflictingWithAppRootDir(Exception inner = null)
		: base("Other app root dir is conflicting with app root dir.", inner)
	{
	}
}
