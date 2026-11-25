using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class NotExistingFile : FunctionException
{
	public Path Path { get; }

	public NotExistingFile(Path path, Exception inner = null)
		: base($"File doesn't exist at '{path}'.", inner)
	{
		Path = path;
	}
}
