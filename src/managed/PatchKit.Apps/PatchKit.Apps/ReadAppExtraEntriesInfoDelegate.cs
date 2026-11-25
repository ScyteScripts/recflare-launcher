using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate ImmutableArray<AppExtraEntryInfo> ReadAppExtraEntriesInfoDelegate(App app, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
