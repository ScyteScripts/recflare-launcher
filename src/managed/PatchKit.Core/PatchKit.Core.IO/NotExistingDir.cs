using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class NotExistingDir : FunctionException
{
	public Path Path { get; }

	public NotExistingDir(Path path, Exception inner = null)
		: base($"Directory doesn't exist at '{path}'.", inner)
	{
		Path = path;
	}
}
