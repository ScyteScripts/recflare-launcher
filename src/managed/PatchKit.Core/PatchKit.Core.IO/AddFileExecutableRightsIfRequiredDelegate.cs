using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object AddFileExecutableRightsIfRequiredDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<NotExistingFile> onNotExistingFile);
