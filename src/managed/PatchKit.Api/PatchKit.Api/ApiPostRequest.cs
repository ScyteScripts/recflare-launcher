namespace PatchKit.Api;

public struct ApiPostRequest
{
	public string Path { get; }

	public string Query { get; }

	public string ValidationError
	{
		get
		{
			if (Path == null)
			{
				return "Path cannot be null.";
			}
			if (Path.StartsWith("/"))
			{
				return "Path cannot start with slash.";
			}
			if (Query == null)
			{
				return "Query cannot be null.";
			}
			return null;
		}
	}

	public ApiPostRequest(string path, string query)
	{
		Path = path;
		Query = query;
	}
}
