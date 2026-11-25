using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate BytesCount GetFileSizeDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
