using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public delegate object CreateAppDirsDelegate(App app, AppVersionId versionId, ImmutableArray<AppPath> dirs, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
