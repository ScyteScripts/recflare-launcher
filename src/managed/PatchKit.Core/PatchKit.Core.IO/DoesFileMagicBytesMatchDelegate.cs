using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate bool DoesFileMagicBytesMatchDelegate(Path path, MagicBytes magicBytes, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
