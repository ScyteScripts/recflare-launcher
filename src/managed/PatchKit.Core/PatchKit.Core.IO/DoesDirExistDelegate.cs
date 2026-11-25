using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate bool DoesDirExistDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
