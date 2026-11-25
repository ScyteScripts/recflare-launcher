using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public delegate HttpResponse GetHttpResponseDelegate(HttpRequestInfo requestInfo, Timeout? timeout, HandleExceptionAndReturnDelegate<ConnectionFailure> onConnectionFailure);
