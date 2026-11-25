using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO.Hashing;

public delegate Hash GetFilexxHashDelegate(Path path, xxHashSeed seed, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
