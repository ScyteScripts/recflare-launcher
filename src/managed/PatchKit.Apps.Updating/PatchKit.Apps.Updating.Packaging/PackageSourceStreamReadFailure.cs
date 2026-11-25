using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Packaging;

public class PackageSourceStreamReadFailure : FunctionException
{
	public PackageSourceStreamReadFailure(Exception inner = null)
		: base("", inner)
	{
	}
}
