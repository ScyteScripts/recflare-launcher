using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public interface IHttpStreamedResponseDisposable : IHttpStreamedResponse, IBaseDisposable, IDisposable
{
}
