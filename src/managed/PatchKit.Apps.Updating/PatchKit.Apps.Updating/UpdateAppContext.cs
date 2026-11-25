using PatchKit.Apps.Remote;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Updating;

public struct UpdateAppContext : IValidatedStruct
{
	public App App { get; }

	public AppRemote AppRemote { get; }

	public AppVersionId TargetVersionId { get; }

	public AppLicenseSecret? LicenseSecret { get; }

	public AppPassword? Password { get; }

	public bool IsValid { get; }

	public static string Validate(App app, AppRemote appRemote, AppVersionId targetVersionId, AppLicenseSecret? licenseSecret, AppPassword? password)
	{
		return null;
	}

	public UpdateAppContext(App app, AppRemote appRemote, AppVersionId targetVersionId, AppLicenseSecret? licenseSecret, AppPassword? password)
	{
		this = default;
		Arg.IsNotNull(app, "app");
		Arg.IsNotNull(appRemote, "appRemote");
		Arg.IsValid(targetVersionId, "targetVersionId");
		Arg.IsValid(licenseSecret, "licenseSecret");
		Arg.IsValid(password, "password");
		string text = Validate(app, appRemote, targetVersionId, licenseSecret, password);
		if (text != null)
		{
			throw new ValidationFailed(text);
		}
		App = app;
		AppRemote = appRemote;
		TargetVersionId = targetVersionId;
		LicenseSecret = licenseSecret;
		Password = password;
		IsValid = true;
	}

	public UpdateAppContext ModifyApp(App newApp)
	{
		return new UpdateAppContext(newApp, AppRemote, TargetVersionId, LicenseSecret, Password);
	}

	public UpdateAppContext ModifyAppRemote(AppRemote newAppRemote)
	{
		return new UpdateAppContext(App, newAppRemote, TargetVersionId, LicenseSecret, Password);
	}

	public UpdateAppContext ModifyTargetVersionId(AppVersionId newTargetVersionId)
	{
		return new UpdateAppContext(App, AppRemote, newTargetVersionId, LicenseSecret, Password);
	}

	public UpdateAppContext ModifyLicenseSecret(AppLicenseSecret? newLicenseSecret)
	{
		return new UpdateAppContext(App, AppRemote, TargetVersionId, newLicenseSecret, Password);
	}

	public UpdateAppContext ModifyPassword(AppPassword? newPassword)
	{
		return new UpdateAppContext(App, AppRemote, TargetVersionId, LicenseSecret, newPassword);
	}

	public override string ToString()
	{
		string text = "{";
		text += $"\"App\" = {App}, \"AppRemote\" = {AppRemote}, \"TargetVersionId\" = {TargetVersionId}, \"LicenseSecret\" = {LicenseSecret}, \"Password\" = {Password}";
		return text + "}";
	}
}
