namespace PatchKit.Api;

public struct ApiConnectionServer(string host, string path, int port, bool useHttps)
{
	public string Host { get; } = host;

	public string Path { get; } = path;

	public int Port { get; } = port;

	public bool UseHttps { get; } = useHttps;

	public string ValidationError
	{
		get
		{
			if (string.IsNullOrEmpty(Host))
			{
				return "Host cannot be null or empty.";
			}
			if (Port < 0)
			{
				return "Port cannot be less than zero.";
			}
			return null;
		}
	}
}
