using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public interface IFileLockDisposable : IFileLock, IBaseDisposable, IDisposable
{
}
