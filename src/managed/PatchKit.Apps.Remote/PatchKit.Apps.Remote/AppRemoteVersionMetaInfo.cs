using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemoteVersionMetaInfo : IValidatedStruct
{
	public Text Label { get; }

	public Text? Changes { get; }

	public AppStartInfo? StartInfo { get; }

	public bool IsValid { get; }

	public static string Validate(Text label, Text? changes, AppStartInfo? startInfo)
	{
		return null;
	}

	public AppRemoteVersionMetaInfo(Text label, Text? changes, AppStartInfo? startInfo)
	{
		this = default;
		Arg.IsValid(label, "label");
		Arg.IsValid(changes, "changes");
		Arg.IsValid(startInfo, "startInfo");
		string text = Validate(label, changes, startInfo);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Label = label;
		Changes = changes;
		StartInfo = startInfo;
		IsValid = true;
	}

	public AppRemoteVersionMetaInfo ModifyLabel(Text newLabel)
	{
		return new AppRemoteVersionMetaInfo(newLabel, Changes, StartInfo);
	}

	public AppRemoteVersionMetaInfo ModifyChanges(Text? newChanges)
	{
		return new AppRemoteVersionMetaInfo(Label, newChanges, StartInfo);
	}

	public AppRemoteVersionMetaInfo ModifyStartInfo(AppStartInfo? newStartInfo)
	{
		return new AppRemoteVersionMetaInfo(Label, Changes, newStartInfo);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Label\" = {Label}, \"Changes\" = {Changes}, \"StartInfo\" = {StartInfo}";
		return text + "}";
	}
}
