using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackageDisposable : IPackage, IBaseDisposable, IDisposable
{
}
