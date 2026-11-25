using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate bool IsFileWindowsExecutableDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
