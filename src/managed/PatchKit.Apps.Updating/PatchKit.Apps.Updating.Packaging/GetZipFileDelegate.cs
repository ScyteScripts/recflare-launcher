using ICSharpCode.SharpZipLib.Zip;
using PatchKit.Apps.Remote;

namespace PatchKit.Apps.Updating.Packaging;

public delegate IPackageFileDisposable GetZipFileDelegate(AppRemotePackageFileInfo info, ZipInputStream stream);
