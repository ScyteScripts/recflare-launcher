using System;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Librsync;

public class NotExistingDiffFile : FunctionException
{
	public Path Path { get; }

	public NotExistingDiffFile(Path path, Exception inner = null)
		: base($"Diff file doesn't exist at '{path}'.", inner)
	{
		Path = path;
	}
}
