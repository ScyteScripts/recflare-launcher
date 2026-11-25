using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public struct HttpRequestInfo : IValidatedStruct
{
	public HttpAddress Address { get; }

	public HttpRequestMethod Method { get; }

	public HttpRequestContent? Content { get; }

	public HttpRequestBytesRange? BytesRange { get; }

	public ImmutableArray<HttpRequestHeader> Headers { get; }

	public bool IsValid { get; }

	public static string Validate(HttpAddress address, HttpRequestMethod method, HttpRequestContent? content, HttpRequestBytesRange? bytesRange, ImmutableArray<HttpRequestHeader> headers)
	{
		if (method == HttpRequestMethod.Get && content.HasValue)
		{
			return "Request cannot contain content if method is set to GET.";
		}
		return null;
	}

	public HttpRequestInfo(HttpAddress address, HttpRequestMethod method, HttpRequestContent? content, HttpRequestBytesRange? bytesRange, ImmutableArray<HttpRequestHeader> headers)
	{
		this = default;
		Arg.IsValid(address, "address");
		Arg.IsValid(content, "content");
		Arg.IsValid(bytesRange, "bytesRange");
		Arg.IsValid(headers, "headers");
		string text = Validate(address, method, content, bytesRange, headers);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		Address = address;
		Method = method;
		Content = content;
		BytesRange = bytesRange;
		Headers = headers;
		IsValid = true;
	}

	public HttpRequestInfo ModifyAddress(HttpAddress newAddress)
	{
		return new HttpRequestInfo(newAddress, Method, Content, BytesRange, Headers);
	}

	public HttpRequestInfo ModifyMethod(HttpRequestMethod newMethod)
	{
		return new HttpRequestInfo(Address, newMethod, Content, BytesRange, Headers);
	}

	public HttpRequestInfo ModifyContent(HttpRequestContent? newContent)
	{
		return new HttpRequestInfo(Address, Method, newContent, BytesRange, Headers);
	}

	public HttpRequestInfo ModifyBytesRange(HttpRequestBytesRange? newBytesRange)
	{
		return new HttpRequestInfo(Address, Method, Content, newBytesRange, Headers);
	}

	public HttpRequestInfo ModifyHeaders(ImmutableArray<HttpRequestHeader> newHeaders)
	{
		return new HttpRequestInfo(Address, Method, Content, BytesRange, newHeaders);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"Address\" = {Address}, \"Method\" = {Method}, \"Content\" = {Content}, \"BytesRange\" = {BytesRange}, \"Headers\" = {Headers}";
		return text + "}";
	}
}
