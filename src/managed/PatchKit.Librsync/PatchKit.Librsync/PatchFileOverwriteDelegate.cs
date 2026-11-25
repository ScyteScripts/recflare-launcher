using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Librsync;

public delegate object PatchFileOverwriteDelegate(Path path, Path diffPath, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<FilePatchingFailure> onFilePatchingFailure, HandleExceptionDelegate<NotExistingFile> onNotExistingFile, HandleExceptionDelegate<NotExistingDiffFile> onNotExistingDiffFile);
