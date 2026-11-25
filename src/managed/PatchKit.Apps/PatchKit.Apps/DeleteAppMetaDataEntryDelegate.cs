using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object DeleteAppMetaDataEntryDelegate(App app, AppPath path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
