using System.Net;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public struct HttpTextResponse : IValidatedStruct
{
	public HttpStatusCode StatusCode { get; }

	public Text Body { get; }

	public bool IsValid { get; }

	public static string Validate(HttpStatusCode statusCode, Text body)
	{
		return null;
	}

	public HttpTextResponse(HttpStatusCode statusCode, Text body)
	{
		this = default;
		Arg.IsValid(body, "body");
		string text = Validate(statusCode, body);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		StatusCode = statusCode;
		Body = body;
		IsValid = true;
	}

	public HttpTextResponse ModifyStatusCode(HttpStatusCode newStatusCode)
	{
		return new HttpTextResponse(newStatusCode, Body);
	}

	public HttpTextResponse ModifyBody(Text newBody)
	{
		return new HttpTextResponse(StatusCode, newBody);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"StatusCode\" = {StatusCode}, \"Body\" = {Body}";
		return text + "}";
	}
}
