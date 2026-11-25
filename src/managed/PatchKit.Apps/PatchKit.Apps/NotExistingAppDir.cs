using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class NotExistingAppDir : FunctionException
{
	public App App { get; }

	public AppPath Path { get; }

	public NotExistingAppDir(App app, AppPath path, Exception inner = null)
		: base($"Directory doesn't exist at '{path}' (app data location: '{app.Path}').", inner)
	{
		App = app;
		Path = path;
	}
}
