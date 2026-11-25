using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object CreateDirDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingFile> onConflictingFile);
