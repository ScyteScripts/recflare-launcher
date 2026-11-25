using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public delegate object DeleteAppExtraEntriesDelegate(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
