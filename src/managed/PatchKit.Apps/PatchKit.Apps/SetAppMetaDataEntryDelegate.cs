using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object SetAppMetaDataEntryDelegate(App app, AppPath path, AppVersionId versionId, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
