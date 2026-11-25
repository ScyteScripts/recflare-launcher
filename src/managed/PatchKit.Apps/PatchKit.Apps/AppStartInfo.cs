using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppStartInfo : IValidatedStruct
{
	public AppPath ExecutablePath { get; }

	public Text? ExecutableArgs { get; }

	public bool IsValid { get; }

	public static string Validate(AppPath executablePath, Text? executableArgs)
	{
		return null;
	}

	public AppStartInfo(AppPath executablePath, Text? executableArgs)
	{
		this = default;
		Arg.IsValid(executablePath, "executablePath");
		Arg.IsValid(executableArgs, "executableArgs");
		string text = Validate(executablePath, executableArgs);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		ExecutablePath = executablePath;
		ExecutableArgs = executableArgs;
		IsValid = true;
	}

	public AppStartInfo ModifyExecutablePath(AppPath newExecutablePath)
	{
		return new AppStartInfo(newExecutablePath, ExecutableArgs);
	}

	public AppStartInfo ModifyExecutableArgs(Text? newExecutableArgs)
	{
		return new AppStartInfo(ExecutablePath, newExecutableArgs);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"ExecutablePath\" = {ExecutablePath}, \"ExecutableArgs\" = {ExecutableArgs}";
		return text + "}";
	}
}
