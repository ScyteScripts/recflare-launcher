using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Network;

public class ConnectionFailure : FunctionException
{
	public ConnectionFailure(Exception inner = null)
		: base("", inner)
	{
	}
}
