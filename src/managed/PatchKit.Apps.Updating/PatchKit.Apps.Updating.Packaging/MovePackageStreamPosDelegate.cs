using System.Threading;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public delegate object MovePackageStreamPosDelegate(IPackageStream stream, BytesCount targetPos, CancellationToken cancellationToken, HandleExceptionDelegate<PackageStreamPosOutOfRange> onPackageStreamPosOutOfRange, HandleExceptionDelegate<InvalidAppAuthKey> onInvalidAppAuthKey, HandleExceptionDelegate<ExpiredAppAuthKey> onExpiredAppAuthKey);
