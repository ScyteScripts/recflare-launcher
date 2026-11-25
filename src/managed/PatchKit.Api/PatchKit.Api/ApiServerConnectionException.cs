using System;

namespace PatchKit.Api;

public class ApiServerConnectionException : Exception
{
	public ApiServerConnectionException(string reason)
		: base(reason)
	{
	}
}
