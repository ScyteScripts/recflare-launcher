using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate ImmutableArray<AppEntryInfo> GetAppEntriesInfoDelegate(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
