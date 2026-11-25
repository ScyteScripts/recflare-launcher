using System.Threading;
using PatchKit.Apps.Updating.Packaging;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Installation;

public delegate object WritePackageFileStreamToFileDelegate(IPackageFileStream packageFileStream, Path filePath, ReportProgressDelegate<BytesCount> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<ConflictingFile> onConflictingFile, HandleExceptionDelegate<ConflictingDir> onConflictingDir, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
