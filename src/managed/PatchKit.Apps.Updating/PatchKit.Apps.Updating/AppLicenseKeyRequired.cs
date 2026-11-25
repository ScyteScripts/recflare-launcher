using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public class AppLicenseKeyRequired : FunctionException
{
	public AppLicenseKeyRequired(Exception inner = null)
		: base("", inner)
	{
	}
}
