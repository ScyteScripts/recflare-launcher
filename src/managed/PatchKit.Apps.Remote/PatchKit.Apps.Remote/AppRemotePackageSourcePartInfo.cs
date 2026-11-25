using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public struct AppRemotePackageSourcePartInfo : IValidatedStruct
{
	public HttpAddress Address { get; }

	public BytesCount Offset { get; }

	public BytesCount Size { get; }

	public Text? AuthHeaderValue { get; }

	public bool IsValid { get; }

	public static string Validate(HttpAddress address, BytesCount offset, BytesCount size, Text? authHeaderValue)
	{
		return null;
	}

	public AppRemotePackageSourcePartInfo(HttpAddress address, BytesCount offset, BytesCount size, Text? authHeaderValue)
	{
		this = default;
		Arg.IsValid(address, "address");
		Arg.IsValid(offset, "offset");
		Arg.IsValid(size, "size");
		Arg.IsValid(authHeaderValue, "authHeaderValue");
		string text = Validate(address, offset, size, authHeaderValue);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Address = address;
		Offset = offset;
		Size = size;
		AuthHeaderValue = authHeaderValue;
		IsValid = true;
	}

	public AppRemotePackageSourcePartInfo ModifyAddress(HttpAddress newAddress)
	{
		return new AppRemotePackageSourcePartInfo(newAddress, Offset, Size, AuthHeaderValue);
	}

	public AppRemotePackageSourcePartInfo ModifyOffset(BytesCount newOffset)
	{
		return new AppRemotePackageSourcePartInfo(Address, newOffset, Size, AuthHeaderValue);
	}

	public AppRemotePackageSourcePartInfo ModifySize(BytesCount newSize)
	{
		return new AppRemotePackageSourcePartInfo(Address, Offset, newSize, AuthHeaderValue);
	}

	public AppRemotePackageSourcePartInfo ModifyAuthHeaderValue(Text? newAuthHeaderValue)
	{
		return new AppRemotePackageSourcePartInfo(Address, Offset, Size, newAuthHeaderValue);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Address\" = {Address}, \"Offset\" = {Offset}, \"Size\" = {Size}, \"AuthHeaderValue\" = {AuthHeaderValue}";
		return text + "}";
	}
}
