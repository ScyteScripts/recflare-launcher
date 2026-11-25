using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object CreateAppDirDelegate(App app, AppPath path, AppVersionId versionId, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingAppDir> onConflictingAppDir, HandleExceptionDelegate<ConflictingAppFile> onConflictingAppFile);
