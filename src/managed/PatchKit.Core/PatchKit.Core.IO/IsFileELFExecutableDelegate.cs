using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate bool IsFileELFExecutableDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
