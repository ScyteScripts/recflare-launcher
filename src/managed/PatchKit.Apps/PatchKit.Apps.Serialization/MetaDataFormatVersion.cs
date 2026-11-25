using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public struct MetaDataFormatVersion : IValidatedStruct
{
	public int Major { get; }

	public int Minor { get; }

	public int Patch { get; }

	public bool IsValid { get; }

	public static string Validate(int major, int minor, int patch)
	{
		if (major < 0)
		{
			return "Major must be equal to 0 or more.";
		}
		if (minor < 0)
		{
			return "Minor must be equal to 0 or more.";
		}
		if (patch < 0)
		{
			return "Patch must be equal to 0 or more.";
		}
		return null;
	}

	public MetaDataFormatVersion(int major, int minor, int patch)
	{
		this = default;
		string text = Validate(major, minor, patch);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Major = major;
		Minor = minor;
		Patch = patch;
		IsValid = true;
	}

	public MetaDataFormatVersion ModifyMajor(int newMajor)
	{
		return new MetaDataFormatVersion(newMajor, Minor, Patch);
	}

	public MetaDataFormatVersion ModifyMinor(int newMinor)
	{
		return new MetaDataFormatVersion(Major, newMinor, Patch);
	}

	public MetaDataFormatVersion ModifyPatch(int newPatch)
	{
		return new MetaDataFormatVersion(Major, Minor, newPatch);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Major\" = {Major}, \"Minor\" = {Minor}, \"Patch\" = {Patch}";
		return text + "}";
	}
}
