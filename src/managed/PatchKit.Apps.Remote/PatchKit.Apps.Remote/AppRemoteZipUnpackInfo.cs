using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemoteZipUnpackInfo : IValidatedStruct
{
	public AppRemoteZipUnpackPassword Password { get; }

	public bool IsValid { get; }

	public static string Validate(AppRemoteZipUnpackPassword password)
	{
		return null;
	}

	public AppRemoteZipUnpackInfo(AppRemoteZipUnpackPassword password)
	{
		this = default;
		Arg.IsValid(password, "password");
		string text = Validate(password);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Password = password;
		IsValid = true;
	}

	public AppRemoteZipUnpackInfo ModifyPassword(AppRemoteZipUnpackPassword newPassword)
	{
		return new AppRemoteZipUnpackInfo(newPassword);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Password\" = {Password}";
		return text + "}";
	}
}
