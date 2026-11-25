using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Network;

public interface IHttpResponseBodyStream : IBaseDisposable
{
	StreamReadResult Read(Buffer<byte> buffer, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure);
}
