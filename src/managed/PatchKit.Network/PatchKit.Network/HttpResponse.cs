using System.Net;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public struct HttpResponse : IValidatedStruct
{
	public HttpStatusCode StatusCode { get; }

	public HttpResponseCharacterSet CharacterSet { get; }

	public ImmutableArray<byte> Body { get; }

	public bool IsValid { get; }

	public static string Validate(HttpStatusCode statusCode, HttpResponseCharacterSet characterSet, ImmutableArray<byte> body)
	{
		return null;
	}

	public HttpResponse(HttpStatusCode statusCode, HttpResponseCharacterSet characterSet, ImmutableArray<byte> body)
	{
		this = default;
		Arg.IsValid(characterSet, "characterSet");
		string text = Validate(statusCode, characterSet, body);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		StatusCode = statusCode;
		CharacterSet = characterSet;
		Body = body;
		IsValid = true;
	}

	public HttpResponse ModifyStatusCode(HttpStatusCode newStatusCode)
	{
		return new HttpResponse(newStatusCode, CharacterSet, Body);
	}

	public HttpResponse ModifyCharacterSet(HttpResponseCharacterSet newCharacterSet)
	{
		return new HttpResponse(StatusCode, newCharacterSet, Body);
	}

	public HttpResponse ModifyBody(ImmutableArray<byte> newBody)
	{
		return new HttpResponse(StatusCode, CharacterSet, newBody);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"StatusCode\" = {StatusCode}, \"CharacterSet\" = {CharacterSet}, \"Body\" = {Body}";
		return text + "}";
	}
}
