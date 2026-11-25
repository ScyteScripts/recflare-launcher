using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate IFileWriteStreamDisposable CreateAppFileDelegate(App app, AppPath path, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionAndReturnDelegate<ConflictingAppFile> onConflictingAppFile, HandleExceptionAndReturnDelegate<ConflictingAppDir> onConflictingAppDir);
