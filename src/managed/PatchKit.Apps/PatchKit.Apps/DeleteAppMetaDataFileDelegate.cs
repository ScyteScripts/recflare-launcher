using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object DeleteAppMetaDataFileDelegate(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
