using System;
using System.Threading;
using Newtonsoft.Json;
using PatchKit.Api.Models;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Api;

public sealed class ApiConnection : IApiConnection
{
	private readonly IBaseApiConnection _baseApiConnection;

	public ApiConnection(string apiCallerId, ApiConnectionSettings settings, BaseApiConnectionFactory baseApiConnectionFactory)
	{
		if (baseApiConnectionFactory == null)
		{
			throw new ArgumentNullException("baseApiConnectionFactory");
		}
		_baseApiConnection = baseApiConnectionFactory(apiCallerId, settings);
	}

	private static void SetPathParam(ref string path, string name, string value)
	{
		path = path.Replace("{" + name + "}", value);
	}

	private static void SetQueryParam(ref string query, string name, string value)
	{
		if (query != string.Empty)
		{
			query += "&";
		}
		query = query + name + "=" + value;
	}

	public ImmutableArray<App> ListUserApplications(string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (apiKey == null)
		{
			throw new ArgumentNullException("apiKey");
		}
		string path = "1/apps";
		string query = string.Empty;
		SetQueryParam(ref query, "api_key", apiKey.ToString());
		return JsonConvert.DeserializeObject<ImmutableArray<App>>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body, new JsonConverter[1]
		{
			new ImmutableArrayJsonConverter<App>()
		});
	}

	public App GetApplicationInfo(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}";
		string empty = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		return JsonConvert.DeserializeObject<App>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body);
	}

	public ImmutableArray<Changelog> GetAppChangelog(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/changelog";
		string empty = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		return JsonConvert.DeserializeObject<ImmutableArray<Changelog>>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body, new JsonConverter[1]
		{
			new ImmutableArrayJsonConverter<Changelog>()
		});
	}

	public ImmutableArray<AppVersion> GetAppVersionList(string appSecret, string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions";
		string query = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		if (apiKey != null)
		{
			SetQueryParam(ref query, "api_key", apiKey.ToString());
		}
		return JsonConvert.DeserializeObject<ImmutableArray<AppVersion>>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body, new JsonConverter[1]
		{
			new ImmutableArrayJsonConverter<AppVersion>()
		});
	}

	public AppVersion GetAppLatestAppVersion(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/latest";
		string empty = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		return JsonConvert.DeserializeObject<AppVersion>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body);
	}

	public AppVersionId GetAppLatestAppVersionId(string appSecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/latest/id";
		string empty = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		return JsonConvert.DeserializeObject<AppVersionId>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body);
	}

	public AppVersion GetAppVersion(string appSecret, int versionId, string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/{version_id}";
		string query = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		SetPathParam(ref path, "version_id", versionId.ToString());
		if (apiKey != null)
		{
			SetQueryParam(ref query, "api_key", apiKey.ToString());
		}
		return JsonConvert.DeserializeObject<AppVersion>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body);
	}

	public AppContentSummary GetAppVersionContentSummary(string appSecret, int versionId, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/{version_id}/content_summary";
		string empty = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		SetPathParam(ref path, "version_id", versionId.ToString());
		return JsonConvert.DeserializeObject<AppContentSummary>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body);
	}

	public AppDiffSummary GetAppVersionDiffSummary(string appSecret, int versionId, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/{version_id}/diff_summary";
		string empty = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		SetPathParam(ref path, "version_id", versionId.ToString());
		return JsonConvert.DeserializeObject<AppDiffSummary>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body);
	}

	public AppDiffSummary GetAppVersionPatchLinkDiffSummary(string appSecret, int versionId, int patchLinkId, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/{version_id}/patch_links/{patch_link_id}/diff_summary";
		string empty = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		SetPathParam(ref path, "version_id", versionId.ToString());
		SetPathParam(ref path, "patch_link_id", patchLinkId.ToString());
		return JsonConvert.DeserializeObject<AppDiffSummary>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body);
	}

	public ImmutableArray<ResourceUrl> GetAppVersionContentUrls(string appSecret, int versionId, string country, string keySecret, string password, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/{version_id}/content_urls";
		string query = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		SetPathParam(ref path, "version_id", versionId.ToString());
		if (country != null)
		{
			SetQueryParam(ref query, "country", country.ToString());
		}
		if (keySecret != null)
		{
			SetQueryParam(ref query, "key_secret", keySecret.ToString());
		}
		if (password != null)
		{
			SetQueryParam(ref query, "password", password.ToString());
		}
		return JsonConvert.DeserializeObject<ImmutableArray<ResourceUrl>>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body, new JsonConverter[1]
		{
			new ImmutableArrayJsonConverter<ResourceUrl>()
		});
	}

	public ImmutableArray<ResourceUrl> GetAppVersionDiffUrls(string appSecret, int versionId, string country, string keySecret, string password, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/{version_id}/diff_urls";
		string query = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		SetPathParam(ref path, "version_id", versionId.ToString());
		if (country != null)
		{
			SetQueryParam(ref query, "country", country.ToString());
		}
		if (keySecret != null)
		{
			SetQueryParam(ref query, "key_secret", keySecret.ToString());
		}
		if (password != null)
		{
			SetQueryParam(ref query, "password", password.ToString());
		}
		return JsonConvert.DeserializeObject<ImmutableArray<ResourceUrl>>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body, new JsonConverter[1]
		{
			new ImmutableArrayJsonConverter<ResourceUrl>()
		});
	}

	public ImmutableArray<ResourceUrl> GetAppVersionPatchLinkDiffUrls(string appSecret, int versionId, int patchLinkId, string country, string keySecret, string password, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/versions/{version_id}/patch_links/{patch_link_id}/diff_urls";
		string query = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		SetPathParam(ref path, "version_id", versionId.ToString());
		SetPathParam(ref path, "patch_link_id", patchLinkId.ToString());
		if (country != null)
		{
			SetQueryParam(ref query, "country", country.ToString());
		}
		if (keySecret != null)
		{
			SetQueryParam(ref query, "key_secret", keySecret.ToString());
		}
		if (password != null)
		{
			SetQueryParam(ref query, "password", password.ToString());
		}
		return JsonConvert.DeserializeObject<ImmutableArray<ResourceUrl>>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body, new JsonConverter[1]
		{
			new ImmutableArrayJsonConverter<ResourceUrl>()
		});
	}

	public AppAuthentication AuthenticateAppWithKey(string appSecret, string keySecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "1/apps/{app_secret}/authenticate_with_key";
		string query = string.Empty;
		SetPathParam(ref path, "app_secret", appSecret.ToString());
		if (keySecret != null)
		{
			SetQueryParam(ref query, "key_secret", keySecret.ToString());
		}
		return JsonConvert.DeserializeObject<AppAuthentication>(_baseApiConnection.SendRequest(new ApiPostRequest(path, query), timeout, cancellationToken).Body);
	}

	public Plan GetPlanInfo(string apiKey, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (apiKey == null)
		{
			throw new ArgumentNullException("apiKey");
		}
		string path = "1/me/plan";
		string query = string.Empty;
		SetQueryParam(ref query, "api_key", apiKey.ToString());
		return JsonConvert.DeserializeObject<Plan>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body);
	}
}
