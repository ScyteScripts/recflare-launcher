using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object DeleteFileIfExistsDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
