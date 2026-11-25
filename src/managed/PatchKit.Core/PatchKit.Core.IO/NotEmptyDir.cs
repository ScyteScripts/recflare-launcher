using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class NotEmptyDir : FunctionException
{
	public Path Path { get; }

	public NotEmptyDir(Path path, Exception inner = null)
		: base($"Cannot finish operation due to not empty directory at '{path}'.", inner)
	{
		Path = path;
	}
}
