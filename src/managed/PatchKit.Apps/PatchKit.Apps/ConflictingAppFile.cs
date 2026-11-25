using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class ConflictingAppFile : FunctionException
{
	public App App { get; }

	public AppPath Path { get; }

	public ConflictingAppFile(App app, AppPath path, Exception inner = null)
		: base($"Unable to finish operation due to conflicting file at `{path}` (app data location: '{app.Path}').", inner)
	{
		App = app;
		Path = path;
	}
}
