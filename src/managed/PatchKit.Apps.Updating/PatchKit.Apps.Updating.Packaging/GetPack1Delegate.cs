using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;
using PatchKit.Core.IO;

namespace PatchKit.Apps.Updating.Packaging;

public delegate IPackageDisposable GetPack1Delegate(ImmutableArray<AppRemotePackageFileInfo> filesInfo, AppRemotePack1UnpackInfo unpackInfo, IPackageStream stream, BytesCount size);
