using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object MoveFileOverwriteDelegate(Path path, Path newPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir);
