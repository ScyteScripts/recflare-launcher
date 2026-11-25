using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackagePatchFileDisposable : IPackageFileDisposable, IPackageFile, IBaseDisposable, IDisposable, IPackagePatchFile
{
}
