using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class NotExistingAppFile : FunctionException
{
	public App App { get; }

	public AppPath Path { get; }

	public NotExistingAppFile(App app, AppPath path, Exception inner = null)
		: base($"File doesn't exist at '{path}' (app data location: '{app.Path}').", inner)
	{
		App = app;
		Path = path;
	}
}
