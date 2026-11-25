using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class FileAlreadyInUse : FunctionException
{
	public Path Path { get; }

	public FileAlreadyInUse(Path path, Exception inner = null)
		: base($"File '{path}' is already in use by a different process.", inner)
	{
		Path = path;
	}
}
