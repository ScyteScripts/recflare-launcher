using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate IFileLockDisposable GetFileLockDelegate(Path path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile, HandleExceptionAndReturnDelegate<FileAlreadyInUse> onFileAlreadyInUse);
