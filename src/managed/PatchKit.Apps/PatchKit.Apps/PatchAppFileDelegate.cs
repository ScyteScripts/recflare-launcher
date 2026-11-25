using PatchKit.Core.CSharp;
using PatchKit.Core.IO;
using PatchKit.Librsync;

namespace PatchKit.Apps;

public delegate object PatchAppFileDelegate(App app, AppPath path, Path patchFilePath, AppVersionPatchId versionPatchId, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingAppFile> onNotExistingAppFile, HandleExceptionDelegate<NotExistingDiffFile> onNotExistingDiffFile, HandleExceptionDelegate<AppFilePatchingFailure> onAppFilePatchingFailure);
