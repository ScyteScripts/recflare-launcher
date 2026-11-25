using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate Text ReadTextFromFileDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
