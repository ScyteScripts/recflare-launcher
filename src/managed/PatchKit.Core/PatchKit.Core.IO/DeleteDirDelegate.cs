using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object DeleteDirDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingDir> onNotExistingDir, HandleExceptionDelegate<NotEmptyDir> onNotEmptyDir);
