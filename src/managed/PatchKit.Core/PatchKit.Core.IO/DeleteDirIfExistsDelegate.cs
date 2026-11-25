using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object DeleteDirIfExistsDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotEmptyDir> onNotEmptyDir);
