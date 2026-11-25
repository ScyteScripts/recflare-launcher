using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object DeleteDirRecursivelyDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingDir> onNotExistingDir);
