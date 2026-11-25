using System;
using System.Net;
using System.Threading;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Network;
using StudioPatch;

namespace PatchKit.Api;

public class BaseApiConnection : IBaseApiConnection
{
	private enum ServerType
	{
		MainServer,
		CacheServer
	}

	private readonly string _apiCallerId;

	private readonly ApiConnectionSettings _settings;

	private readonly IApiCache _apiCache;

	private readonly LogDelegate _log;

	private GetHttpTextResponseDelegate GetHttpTextResponse { get; }

	private void LogDebug(string description, Exception exception = null)
	{
		_log(new LogMessage(description, LogMessageType.Info, exception));
	}

	private void LogTrace(string description, Exception exception = null)
	{
		_log(new LogMessage(description, LogMessageType.Trace, exception));
	}

	private void LogWarning(string description, Exception exception = null)
	{
		_log(new LogMessage(description, LogMessageType.Warning, exception));
	}

	private void LogError(string description, Exception exception = null)
	{
		_log(new LogMessage(description, LogMessageType.Error, exception));
	}

	public BaseApiConnection(string apiCallerId, ApiConnectionSettings settings, GetHttpTextResponseDelegate getHttpTextResponse, IApiCache apiCache, LogDelegate log)
	{
		_apiCallerId = apiCallerId;
		_settings = settings;
		GetHttpTextResponse = getHttpTextResponse ?? throw new ArgumentNullException("getHttpTextResponse");
		_apiCache = apiCache ?? throw new ArgumentNullException("apiCache");
		_log = log ?? throw new ArgumentNullException("log");
	}

	private PatchKit.Network.HttpRequestHeader GetApiCallerIdHeader()
	{
		return new PatchKit.Network.HttpRequestHeader(new Text("PatchKitCaller"), new Text(_apiCallerId));
	}

	private PatchKit.Network.HttpRequestHeader GetUserAgentHeader()
	{
		return new PatchKit.Network.HttpRequestHeader(new Text("User-Agent"), new Text("libpkapps/1.0"));
	}

	public ApiResponse SendRequest(ApiGetRequest request, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		LogDebug("Sending GET request with path: '" + request.Path + "' and query: '" + request.Query + "'...");
		ApiResponse? apiResponse = _apiCache.Retrieve(request);
		if (apiResponse.HasValue)
		{
			LogDebug("Response found in cache No need to send request again.");
			LogTrace("Cached response body: " + apiResponse.Value.Body);
			return apiResponse.Value;
		}
		ApiResponse apiResponse2 = SendRequest(SendRequestToServer, useCacheServers: true);
		_apiCache.Save(request, apiResponse2);
		return apiResponse2;
		HttpTextResponse SendRequestToServer(ApiConnectionServer server)
		{
			HttpAddress address = PrepareAddress(server, request.Path, request.Query);
			return GetHttpTextResponse(new HttpRequestInfo(address, HttpRequestMethod.Get, null, null, new PatchKit.Network.HttpRequestHeader[2]
			{
				GetApiCallerIdHeader(),
				GetUserAgentHeader()
			}.AsImmutable()), timeout, null);
		}
	}

	public ApiResponse SendRequest(ApiPostRequest request, PatchKit.Core.Timeout? timeout, CancellationToken cancellationToken)
	{
		LogDebug("Sending POST request with path: '" + request.Path + "' and query: '" + request.Query + "'...");
		return SendRequest(SendRequestToServer, useCacheServers: false);
		HttpTextResponse SendRequestToServer(ApiConnectionServer server)
		{
			HttpAddress address = PrepareAddress(server, request.Path, request.Query);
			return GetHttpTextResponse(new HttpRequestInfo(address, HttpRequestMethod.Post, null, null, new PatchKit.Network.HttpRequestHeader[2]
			{
				GetApiCallerIdHeader(),
				GetUserAgentHeader()
			}.AsImmutable()), timeout, null);
		}
	}

	private ApiResponse SendRequest(ApiConnectionServer server, Func<ApiConnectionServer, HttpTextResponse> sendRequest, ServerType serverType)
	{
		LogDebug($"Sending request to server ({serverType}): '{server.Host}/{server.Path}:{server.Port}' (uses HTTPS: {server.UseHttps})...");
		HttpTextResponse httpResponse = sendRequest(server);
		LogDebug("Received response. Checking whether it is valid...");
		LogTrace($"Response status code: {httpResponse.StatusCode}");
		if (IsResponseValid(httpResponse, serverType))
		{
			ApiResponse result = new ApiResponse(httpResponse.Body);
			LogDebug("Successfully got response.");
			LogTrace("Response body: " + result.Body);
			return result;
		}
		LogWarning("Response is not valid.");
		if (IsResponseUnexpectedError(httpResponse, serverType))
		{
			throw new ApiResponseException((int)httpResponse.StatusCode);
		}
		throw new ApiServerConnectionException($"Server '{server.Host}/{server.Path}' returned code {(int)httpResponse.StatusCode}");
	}

	private ApiResponse SendRequest(Func<ApiConnectionServer, HttpTextResponse> sendRequest, bool useCacheServers)
	{
		try
		{
			Exception mainServerException;
			try
			{
				return SendRequest(_settings.MainServer, sendRequest, ServerType.MainServer);
			}
			catch (ConnectionFailure connectionFailure)
			{
				LogWarning("Error while connecting to main API server.", connectionFailure);
				mainServerException = connectionFailure;
			}
			catch (ApiServerConnectionException ex)
			{
				LogWarning("Error while connecting to main API server.", ex);
				mainServerException = ex;
			}
			Exception[] array = new Exception[_settings.CacheServers.Length];
			if (useCacheServers)
			{
				LogDebug("Trying to use cache servers...");
				for (int i = 0; i < _settings.CacheServers.Length; i++)
				{
					try
					{
						return SendRequest(_settings.CacheServers[i], sendRequest, ServerType.CacheServer);
					}
					catch (ConnectionFailure connectionFailure2)
					{
						LogWarning("Error while connecting to cache API server.", connectionFailure2);
						array[i] = connectionFailure2;
					}
					catch (ApiServerConnectionException ex2)
					{
						LogWarning("Error while connecting to cache API server.", ex2);
						array[i] = ex2;
					}
				}
			}
			throw new ApiConnectionException(mainServerException, array.AsImmutable());
		}
		catch (Exception exception)
		{
			LogError("Failed to send request.", exception);
			throw;
		}
	}

	private static bool IsResponseValid(HttpTextResponse httpResponse, ServerType serverType)
	{
		return serverType switch
		{
			ServerType.MainServer => IsStatusCodeOk(httpResponse.StatusCode), 
			ServerType.CacheServer => httpResponse.StatusCode == HttpStatusCode.OK, 
			_ => throw new ArgumentOutOfRangeException("serverType", serverType, null), 
		};
	}

	private static bool IsResponseUnexpectedError(HttpTextResponse httpResponse, ServerType serverType)
	{
		return serverType switch
		{
			ServerType.MainServer => !IsStatusCodeOk(httpResponse.StatusCode) && !IsStatusCodeServerError(httpResponse.StatusCode), 
			ServerType.CacheServer => false, 
			_ => throw new ArgumentOutOfRangeException("serverType", serverType, null), 
		};
	}

	private static HttpAddress PrepareAddress(ApiConnectionServer server, string path, string query)
	{
		return new HttpAddress(ServiceRouting.ResolveUri(new UriBuilder
		{
			Host = server.Host,
			Path = ((server.Path == null || server.Path == "/") ? path : (server.Path + "/" + path)),
			Query = query,
			Port = server.Port,
			Scheme = (server.UseHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)
		}.Uri));
	}

	private static bool IsStatusCodeOk(HttpStatusCode statusCode)
	{
		return IsWithin((int)statusCode, 200, 299);
	}

	private static bool IsStatusCodeServerError(HttpStatusCode statusCode)
	{
		return IsWithin((int)statusCode, 500, 599);
	}

	private static bool IsWithin(int value, int min, int max)
	{
		return value >= min && value <= max;
	}
}
