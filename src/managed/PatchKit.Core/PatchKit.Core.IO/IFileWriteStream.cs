using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public interface IFileWriteStream : IBaseDisposable
{
	void Write(Buffer<byte> buffer, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace);
}
