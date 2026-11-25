using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object MoveDirDelegate(Path path, Path newPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingDir> onNotExistingDir, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<NotEmptyDir> onNotEmptyDir);
