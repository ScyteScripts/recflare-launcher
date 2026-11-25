using System;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class AppFilePatchingFailure : FunctionException
{
	public App App { get; }

	public AppPath Path { get; }

	public Path PatchFilePath { get; }

	public AppFilePatchingFailure(App app, AppPath path, Path patchFilePath, Exception inner = null)
		: base($"Patching '{path}' with '{patchFilePath}' failed.", inner)
	{
		App = app;
		Path = path;
		PatchFilePath = patchFilePath;
	}
}
