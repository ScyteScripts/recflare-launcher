using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate bool DoesFileExistDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
