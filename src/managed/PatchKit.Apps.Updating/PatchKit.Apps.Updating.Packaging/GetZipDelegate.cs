using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public delegate IPackageDisposable GetZipDelegate(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemoteZipUnpackInfo unpackInfo, IPackageStream stream, BytesCount size);
