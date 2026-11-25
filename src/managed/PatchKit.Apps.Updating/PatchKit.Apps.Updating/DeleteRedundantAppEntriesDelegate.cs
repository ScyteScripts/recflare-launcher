using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public delegate object DeleteRedundantAppEntriesDelegate(App app, ImmutableArray<AppPath> entries, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
