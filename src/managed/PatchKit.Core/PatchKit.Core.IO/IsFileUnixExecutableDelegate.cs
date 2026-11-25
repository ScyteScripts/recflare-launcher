using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate bool IsFileUnixExecutableDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
