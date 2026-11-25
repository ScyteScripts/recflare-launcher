using System;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public class BlockedAppLicenseKey : FunctionException
{
	public AppLicenseKey LicenseKey { get; }

	public BlockedAppLicenseKey(AppLicenseKey licenseKey, Exception inner = null)
		: base($"{licenseKey} is blocked.", inner)
	{
		LicenseKey = licenseKey;
	}
}
