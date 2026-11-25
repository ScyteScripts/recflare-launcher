using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object WriteTextToFileDelegate(Path path, Text text, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingDir> onConflictingDir, HandleExceptionDelegate<ConflictingFile> onConflictingFile);
