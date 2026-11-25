using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Network;

public interface IHttpRequestContentStream : IBaseDisposable
{
	BytesCount Length { get; }

	StreamReadResult Read(Buffer<byte> buffer);
}
