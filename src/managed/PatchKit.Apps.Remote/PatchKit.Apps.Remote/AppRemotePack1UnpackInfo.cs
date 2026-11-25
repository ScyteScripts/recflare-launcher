using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public struct AppRemotePack1UnpackInfo : IValidatedStruct
{
	public ImmutableArray<AppRemotePack1FileUnpackInfo> FilesInfo { get; }

	public AppRemotePack1Encryption Encryption { get; }

	public AppRemotePack1CompressionMethod CompressionMethod { get; }

	public AppRemotePack1UnpackPassword Password { get; }

	public bool IsValid { get; }

	public static string Validate(ImmutableArray<AppRemotePack1FileUnpackInfo> filesInfo, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password)
	{
		return null;
	}

	public AppRemotePack1UnpackInfo(ImmutableArray<AppRemotePack1FileUnpackInfo> filesInfo, AppRemotePack1Encryption encryption, AppRemotePack1CompressionMethod compressionMethod, AppRemotePack1UnpackPassword password)
	{
		this = default;
		Arg.IsValid(filesInfo, "filesInfo");
		Arg.IsValid(encryption, "encryption");
		Arg.IsValid(password, "password");
		string text = Validate(filesInfo, encryption, compressionMethod, password);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		FilesInfo = filesInfo;
		Encryption = encryption;
		CompressionMethod = compressionMethod;
		Password = password;
		IsValid = true;
	}

	public AppRemotePack1UnpackInfo ModifyFilesInfo(ImmutableArray<AppRemotePack1FileUnpackInfo> newFilesInfo)
	{
		return new AppRemotePack1UnpackInfo(newFilesInfo, Encryption, CompressionMethod, Password);
	}

	public AppRemotePack1UnpackInfo ModifyEncryption(AppRemotePack1Encryption newEncryption)
	{
		return new AppRemotePack1UnpackInfo(FilesInfo, newEncryption, CompressionMethod, Password);
	}

	public AppRemotePack1UnpackInfo ModifyCompressionMethod(AppRemotePack1CompressionMethod newCompressionMethod)
	{
		return new AppRemotePack1UnpackInfo(FilesInfo, Encryption, newCompressionMethod, Password);
	}

	public AppRemotePack1UnpackInfo ModifyPassword(AppRemotePack1UnpackPassword newPassword)
	{
		return new AppRemotePack1UnpackInfo(FilesInfo, Encryption, CompressionMethod, newPassword);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"FilesInfo\" = {FilesInfo}, \"Encryption\" = {Encryption}, \"CompressionMethod\" = {CompressionMethod}, \"Password\" = {Password}";
		return text + "}";
	}
}
