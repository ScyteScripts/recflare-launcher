using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public interface IBufferedHttpStreamedResponseDisposable : IBufferedHttpStreamedResponse, IBaseDisposable, IDisposable
{
}
