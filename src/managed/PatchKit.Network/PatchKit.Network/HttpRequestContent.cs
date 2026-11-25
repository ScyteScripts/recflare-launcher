using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public struct HttpRequestContent : IValidatedStruct
{
	public HttpRequestContentType Type { get; }

	public IHttpRequestContentStream Stream { get; }

	public bool IsValid { get; }

	public static string Validate(HttpRequestContentType type, IHttpRequestContentStream stream)
	{
		return null;
	}

	public HttpRequestContent(HttpRequestContentType type, IHttpRequestContentStream stream)
	{
		this = default;
		Arg.IsNotNull(stream, "stream");
		string text = Validate(type, stream);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Type = type;
		Stream = stream;
		IsValid = true;
	}

	public HttpRequestContent ModifyType(HttpRequestContentType newType)
	{
		return new HttpRequestContent(newType, Stream);
	}

	public HttpRequestContent ModifyStream(IHttpRequestContentStream newStream)
	{
		return new HttpRequestContent(Type, newStream);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Type\" = {Type}, \"Stream\" = {Stream}";
		return text + "}";
	}
}
