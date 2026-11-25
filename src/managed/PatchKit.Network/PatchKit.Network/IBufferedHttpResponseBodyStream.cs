using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Network;

public interface IBufferedHttpResponseBodyStream : IHttpResponseBodyStream, IBaseDisposable
{
	BytesCount GetBufferedBytes();
}
