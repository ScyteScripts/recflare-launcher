using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public struct AppMetaData : IValidatedStruct
{
	public AppInfo Info { get; }

	public ImmutableArray<AppMetaDataEntry> Entries { get; }

	public bool IsValid { get; }

	public static string Validate(AppInfo info, ImmutableArray<AppMetaDataEntry> entries)
	{
		return null;
	}

	public AppMetaData(AppInfo info, ImmutableArray<AppMetaDataEntry> entries)
	{
		this = default;
		Arg.IsValid(info, "info");
		Arg.IsValid(entries, "entries");
		string text = Validate(info, entries);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Info = info;
		Entries = entries;
		IsValid = true;
	}

	public AppMetaData ModifyInfo(AppInfo newInfo)
	{
		return new AppMetaData(newInfo, Entries);
	}

	public AppMetaData ModifyEntries(ImmutableArray<AppMetaDataEntry> newEntries)
	{
		return new AppMetaData(Info, newEntries);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Info\" = {Info}, \"Entries\" = {Entries}";
		return text + "}";
	}
}
