using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object CreateParentDirDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingFile> onConflictingFile);
