using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate ImmutableArray<AppPath> GetAppFilePathsDelegate(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
