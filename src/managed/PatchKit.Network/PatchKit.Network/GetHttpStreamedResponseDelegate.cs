using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public delegate IHttpStreamedResponseDisposable GetHttpStreamedResponseDelegate(HttpRequestInfo requestInfo, Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure);
