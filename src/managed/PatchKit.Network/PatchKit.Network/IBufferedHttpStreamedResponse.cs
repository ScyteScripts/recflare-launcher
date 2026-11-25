using System.Net;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public interface IBufferedHttpStreamedResponse : IBaseDisposable
{
	HttpStatusCode StatusCode { get; }

	HttpResponseCharacterSet CharacterSet { get; }

	IBufferedHttpResponseBodyStream BodyStream { get; }
}
