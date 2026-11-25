using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class AppRemoteUnauthorizedAccess : FunctionException
{
	public AppRemote AppRemote { get; }

	public AppRemoteUnauthorizedAccess(AppRemote appRemote, Exception inner = null)
		: base($"Unauthorized access to app remote with secret {appRemote.Secret}.", inner)
	{
		AppRemote = appRemote;
	}
}
