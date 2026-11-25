using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating.Installation;

public class InconsistentInstallInfo : FunctionException
{
	public InconsistentInstallInfo(Exception inner = null)
		: base("", inner)
	{
	}
}
