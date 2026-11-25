namespace PatchKit.Api;

public delegate IBaseApiConnection BaseApiConnectionFactory(string apiCallerId, ApiConnectionSettings settings);
