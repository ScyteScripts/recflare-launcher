using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public delegate ITempDirDisposable GetTempDirDelegate(HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
