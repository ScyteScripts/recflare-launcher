using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public interface IPack1FileSourceStreamDisposable : IPack1FileSourceStream, IBaseDisposable, IDisposable
{
}
