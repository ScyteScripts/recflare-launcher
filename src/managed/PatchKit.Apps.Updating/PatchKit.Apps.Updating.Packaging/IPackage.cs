using System.Threading;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackage : IBaseDisposable
{
	BytesCount TotalBytes { get; }

	IPackageFileDisposable GetNextFile(CancellationToken cancellationToken, HandleExceptionAndReturnDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionAndReturnDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);

	BytesCount GetProcessedBytes();
}
