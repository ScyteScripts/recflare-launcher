using System.Threading;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackageMultisourceStream : IBaseDisposable
{
	StreamReadResult Read(Buffer<byte> buffer, CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);

	BytesCount GetBufferedBytes();
}
