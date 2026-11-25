using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object MoveFileDelegate(Path path, Path newPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir);
