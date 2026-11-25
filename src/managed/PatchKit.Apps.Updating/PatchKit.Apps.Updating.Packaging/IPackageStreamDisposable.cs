using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackageStreamDisposable : IPackageStream, IBaseDisposable, IDisposable
{
}
