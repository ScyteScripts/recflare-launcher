using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Remote;

public class AppRemoteNotFound : FunctionException
{
	public AppSecret Secret { get; }

	public AppRemoteNotFound(AppSecret secret, Exception inner = null)
		: base($"App remote with secret {secret} not found.", inner)
	{
		Secret = secret;
	}
}
