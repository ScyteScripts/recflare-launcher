using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public interface IHttpRequestContentStreamDisposable : IHttpRequestContentStream, IBaseDisposable, IDisposable
{
}
