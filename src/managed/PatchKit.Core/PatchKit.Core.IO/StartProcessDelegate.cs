using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate ProcessId StartProcessDelegate(Path executablePath, Text? executableArgs, Path workingDirPath, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<NotExistingFile> onNotExistingFile);
