using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Core.IO;

public interface IFileWriteStreamDisposable : IFileWriteStream, IBaseDisposable, IDisposable
{
}
