using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePack1AesEncryption : IValidatedStruct
{
	public Text InitializationVector { get; }

	public bool IsValid { get; }

	public static string Validate(Text initializationVector)
	{
		return null;
	}

	public AppRemotePack1AesEncryption(Text initializationVector)
	{
		this = default;
		Arg.IsValid(initializationVector, "initializationVector");
		string text = Validate(initializationVector);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		InitializationVector = initializationVector;
		IsValid = true;
	}

	public AppRemotePack1AesEncryption ModifyInitializationVector(Text newInitializationVector)
	{
		return new AppRemotePack1AesEncryption(newInitializationVector);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"InitializationVector\" = {InitializationVector}";
		return text + "}";
	}
}
