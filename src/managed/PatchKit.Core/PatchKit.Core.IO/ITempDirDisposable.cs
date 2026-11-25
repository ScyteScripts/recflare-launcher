using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public interface ITempDirDisposable : ITempDir, IBaseDisposable, IDisposable
{
}
