using System.Net;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public interface IHttpStreamedResponse : IBaseDisposable
{
	HttpStatusCode StatusCode { get; }

	HttpResponseCharacterSet CharacterSet { get; }

	IHttpResponseBodyStream BodyStream { get; }
}
