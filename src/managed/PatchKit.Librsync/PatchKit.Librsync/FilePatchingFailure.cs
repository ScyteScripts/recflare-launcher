using System;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Librsync;

public class FilePatchingFailure : FunctionException
{
	public Path Path { get; }

	public Path DiffPath { get; }

	public Path TargetPath { get; }

	public int ErrorCode { get; }

	public FilePatchingFailure(Path path, Path diffPath, Path targetPath, int errorCode, Exception inner = null)
		: base($"librsync patching '{path}' with '{diffPath}' to '{targetPath}' resulted in error code: {errorCode}", inner)
	{
		Path = path;
		DiffPath = diffPath;
		TargetPath = targetPath;
		ErrorCode = errorCode;
	}
}
