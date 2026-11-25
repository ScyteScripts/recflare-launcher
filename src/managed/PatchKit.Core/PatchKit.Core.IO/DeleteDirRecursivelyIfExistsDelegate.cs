using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object DeleteDirRecursivelyIfExistsDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
