using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePack1Encryption : IValidatedStruct
{
	public AppRemotePack1AesEncryption? Aes { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemotePack1AesEncryption? aes)
	{
		if (!aes.HasValue)
		{
			return "Value must be provided.";
		}
		return null;
	}

	public AppRemotePack1Encryption(AppRemotePack1AesEncryption? aes)
	{
		this = default;
		Arg.IsValid(aes, "aes");
		string text = Validate(aes);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Aes = aes;
		IsValid = true;
	}

	public AppRemotePack1Encryption ModifyAes(AppRemotePack1AesEncryption? newAes)
	{
		return new AppRemotePack1Encryption(newAes);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Aes\" = {Aes}";
		return text + "}";
	}
}
