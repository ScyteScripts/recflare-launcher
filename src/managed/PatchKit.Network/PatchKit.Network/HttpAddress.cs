using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public struct HttpAddress : IValidatedStruct
{
	public Uri Value { get; }

	public bool IsValid { get; }

	public static string Validate(Uri value)
	{
		if (value.Scheme != Uri.UriSchemeHttp && value.Scheme != Uri.UriSchemeHttps)
		{
			return "Uri scheme must be either HTTP or HTTPS.";
		}
		return null;
	}

	public HttpAddress(Uri v)
	{
		this = default;
		Arg.IsNotNull(v, "v");
		string text = Validate(v);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Value = v;
		IsValid = true;
	}

	public static implicit operator Uri(HttpAddress @this)
	{
		return @this.Value;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
