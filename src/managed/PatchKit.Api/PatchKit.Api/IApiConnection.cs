using System.Threading;
using PatchKit.Api.Models;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Api;

public interface IApiConnection
{
	ImmutableArray<App> ListUserApplications(string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	App GetApplicationInfo(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	ImmutableArray<Changelog> GetAppChangelog(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	ImmutableArray<AppVersion> GetAppVersionList(string appSecret, string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	AppVersion GetAppLatestAppVersion(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	AppVersionId GetAppLatestAppVersionId(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	AppVersion GetAppVersion(string appSecret, int versionId, string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	AppContentSummary GetAppVersionContentSummary(string appSecret, int versionId, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	AppDiffSummary GetAppVersionDiffSummary(string appSecret, int versionId, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	AppDiffSummary GetAppVersionPatchLinkDiffSummary(string appSecret, int versionId, int patchLinkId, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	ImmutableArray<ResourceUrl> GetAppVersionContentUrls(string appSecret, int versionId, string country, string keySecret, string password, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	ImmutableArray<ResourceUrl> GetAppVersionDiffUrls(string appSecret, int versionId, string country, string keySecret, string password, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	ImmutableArray<ResourceUrl> GetAppVersionPatchLinkDiffUrls(string appSecret, int versionId, int patchLinkId, string country, string keySecret, string password, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	AppAuthentication AuthenticateAppWithKey(string appSecret, string keySecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);

	Plan GetPlanInfo(string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken);
}
