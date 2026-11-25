using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class ConflictingFile : FunctionException
{
	public Path Path { get; }

	public ConflictingFile(Path path, Exception inner = null)
		: base($"Cannot finish operation due to conflicting file at '{path}'.", inner)
	{
		Path = path;
	}
}
