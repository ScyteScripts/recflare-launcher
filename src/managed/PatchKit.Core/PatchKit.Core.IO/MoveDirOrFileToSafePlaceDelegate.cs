using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate object MoveDirOrFileToSafePlaceDelegate(Path path, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess);
