using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public interface IBufferedHttpResponseBodyStreamDisposable : IHttpResponseBodyStreamDisposable, IHttpResponseBodyStream, IBaseDisposable, IDisposable, IBufferedHttpResponseBodyStream
{
}
