using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Strategy;

public delegate UpdateFileState? GetUpdateFileStateDelegate(App app, AppPath path, AppValidationMode validationMode, HandleExceptionAndReturnDelegate<UnauthorizedAccess> onUnauthorizedAccess);
