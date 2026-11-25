using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public interface IHttpResponseBodyStreamDisposable : IHttpResponseBodyStream, IBaseDisposable, IDisposable
{
}
