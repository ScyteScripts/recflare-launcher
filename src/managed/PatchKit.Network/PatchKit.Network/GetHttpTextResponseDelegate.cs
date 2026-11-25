using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public delegate HttpTextResponse GetHttpTextResponseDelegate(HttpRequestInfo requestInfo, Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure);
