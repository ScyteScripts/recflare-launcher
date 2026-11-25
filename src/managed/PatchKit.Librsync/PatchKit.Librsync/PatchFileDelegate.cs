using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Librsync;

public delegate object PatchFileDelegate(Path path, Path diffPath, Path targetPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<FilePatchingFailure> onFilePatchingFailure, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<NotExistingDiffFile> onNotExistingDiffFile, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir);
