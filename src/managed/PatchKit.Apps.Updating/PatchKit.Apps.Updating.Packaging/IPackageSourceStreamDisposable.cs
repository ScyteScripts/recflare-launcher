using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPackageSourceStreamDisposable : IPackageSourceStream, IBaseDisposable, IDisposable
{
}
