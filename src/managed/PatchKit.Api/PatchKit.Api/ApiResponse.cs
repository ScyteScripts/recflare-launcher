namespace PatchKit.Api;

public struct ApiResponse(string body)
{
	public string Body { get; } = body;

	public string ValidationError
	{
		get
		{
			if (Body == null)
			{
				return "Body cannot be null.";
			}
			return null;
		}
	}
}
