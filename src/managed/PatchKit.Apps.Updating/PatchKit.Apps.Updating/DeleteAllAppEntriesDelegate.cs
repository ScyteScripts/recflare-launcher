using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public delegate object DeleteAllAppEntriesDelegate(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
