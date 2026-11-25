using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public delegate IBufferedHttpStreamedResponseDisposable GetBufferedHttpStreamedResponseDelegate(HttpRequestInfo requestInfo, Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure);
