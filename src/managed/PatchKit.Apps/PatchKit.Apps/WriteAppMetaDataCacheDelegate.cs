using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps;

public delegate object WriteAppMetaDataCacheDelegate(App app, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
