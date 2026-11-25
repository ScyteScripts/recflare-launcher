using System.Threading;
using PatchKit.Apps.Remote;
using PatchKit.Apps.Updating.Strategy;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating;

public delegate object UpdateAppFilesDelegate(UpdateAppContext context, ImmutableArray<FileUpdateStrategy> strategy, ReportProgressDelegate<UpdateAppProgress> reportProgress, CancellationToken cancellationToken, HandleExceptionDelegate<UnauthorizedAccess> onUnauthorizedAccess, HandleExceptionDelegate<UpdateAppFailure> onUpdateAppFailure, HandleExceptionDelegate<OutOfFreeDiskSpace> onOutOfFreeDiskSpace, HandleExceptionDelegate<AppRemoteVersionNotFound> onAppRemoteVersionNotFound, HandleExceptionDelegate<InvalidAppPassword> onInvalidAppPassword, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
