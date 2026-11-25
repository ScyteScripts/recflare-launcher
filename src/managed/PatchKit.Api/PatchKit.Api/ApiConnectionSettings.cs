using PatchKit.Core.CSharp;

namespace PatchKit.Api;

public struct ApiConnectionSettings(ApiConnectionServer mainServer, ImmutableArray<ApiConnectionServer> cacheServers)
{
	public ApiConnectionServer MainServer { get; } = mainServer;

	public ImmutableArray<ApiConnectionServer> CacheServers { get; } = cacheServers;
}
