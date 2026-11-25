using System;
using System.Threading;
using Newtonsoft.Json;
using PatchKit.Api.Models.Keys;
using PatchKit.Core;

namespace PatchKit.Api;

public sealed class KeysApiConnection : IKeysApiConnection
{
	private readonly IBaseApiConnection _baseApiConnection;

	public KeysApiConnection(string apiCallerId, ApiConnectionSettings settings, BaseApiConnectionFactory baseApiConnectionFactory)
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

	public Job GetJobInfo(string guid, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (guid == null)
		{
			throw new ArgumentNullException("guid");
		}
		string path = "v2/jobs/{guid}";
		string empty = string.Empty;
		SetPathParam(ref path, "guid", guid.ToString());
		return JsonConvert.DeserializeObject<Job>(_baseApiConnection.SendRequest(new ApiGetRequest(path, empty), timeout, cancellationToken).Body);
	}

	public LicenseKeysCollection GetCollectionKeys(int collectionId, string token, int size, int offset, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (token == null)
		{
			throw new ArgumentNullException("token");
		}
		string path = "v2/collections/{collection_id}/keys";
		string query = string.Empty;
		SetPathParam(ref path, "collection_id", collectionId.ToString());
		SetQueryParam(ref query, "size", size.ToString());
		SetQueryParam(ref query, "offset", offset.ToString());
		return JsonConvert.DeserializeObject<LicenseKeysCollection>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body);
	}

	public LicenseKey GetKeyInfo(string key, string appSecret, string keySecret, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (appSecret == null)
		{
			throw new ArgumentNullException("appSecret");
		}
		string path = "v2/keys/{key}";
		string query = string.Empty;
		SetPathParam(ref path, "key", key.ToString());
		SetQueryParam(ref query, "app_secret", appSecret.ToString());
		if (keySecret != null)
		{
			SetQueryParam(ref query, "key_secret", keySecret.ToString());
		}
		return JsonConvert.DeserializeObject<LicenseKey>(_baseApiConnection.SendRequest(new ApiGetRequest(path, query), timeout, cancellationToken).Body);
	}
}
