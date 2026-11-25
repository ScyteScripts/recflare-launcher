using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public class InvalidAppLicenseKey : FunctionException
{
	public AppLicenseKey LicenseKey { get; }

	public InvalidAppLicenseKey(AppLicenseKey licenseKey, Exception inner = null)
		: base($"{licenseKey} is not valid.", inner)
	{
		LicenseKey = licenseKey;
	}
}
