using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate bool IsDirEmptyDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingDir> onNotExistingDir);
