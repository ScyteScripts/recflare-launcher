using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class FileIsConflictingWithAppRootDir : FunctionException
{
	public FileIsConflictingWithAppRootDir(Exception inner = null)
		: base("File is conflicting with app root dir.", inner)
	{
	}
}
