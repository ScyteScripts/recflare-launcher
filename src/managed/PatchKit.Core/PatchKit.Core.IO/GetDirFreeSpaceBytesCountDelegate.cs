using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate BytesCount GetDirFreeSpaceBytesCountDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
