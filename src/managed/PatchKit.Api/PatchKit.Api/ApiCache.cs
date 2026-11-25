using System.Collections.Generic;

namespace PatchKit.Api;

public class ApiCache : IApiCache
{
	private readonly Dictionary<ApiGetRequest, ApiResponse> _data = new Dictionary<ApiGetRequest, ApiResponse>();

	public void Save(ApiGetRequest request, ApiResponse response)
	{
		_data[request] = response;
	}

	public ApiResponse? Retrieve(ApiGetRequest request)
	{
		if (!_data.ContainsKey(request))
		{
			return null;
		}
		return _data[request];
	}
}
