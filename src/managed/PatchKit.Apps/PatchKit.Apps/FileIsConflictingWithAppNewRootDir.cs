using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class FileIsConflictingWithAppNewRootDir : FunctionException
{
	public FileIsConflictingWithAppNewRootDir(Exception inner = null)
		: base("File is conflicting with app new root dir.", inner)
	{
	}
}
