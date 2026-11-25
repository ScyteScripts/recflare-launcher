using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object CopyFileDelegate(Path sourcePath, Path targetPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir);
