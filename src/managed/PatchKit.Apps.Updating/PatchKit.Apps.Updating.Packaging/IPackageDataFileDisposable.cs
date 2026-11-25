using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackageDataFileDisposable : IPackageFileDisposable, IPackageFile, IBaseDisposable, IDisposable, IPackageDataFile
{
}
