using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public class ConflictingDir : FunctionException
{
	public Path Path { get; }

	public ConflictingDir(Path path, Exception inner = null)
		: base($"Cannot finish operation due to conflicting directory at '{path}'.", inner)
	{
		Path = path;
	}
}
