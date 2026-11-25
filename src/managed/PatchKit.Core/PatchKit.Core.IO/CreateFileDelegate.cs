using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate IFileWriteStreamDisposable CreateFileDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<ConflictingFile> onConflictingFile, HandleExceptionAndReturnDelegate<ConflictingDir> onConflictingDir);
