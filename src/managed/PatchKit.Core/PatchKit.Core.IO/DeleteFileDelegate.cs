using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object DeleteFileDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile);
