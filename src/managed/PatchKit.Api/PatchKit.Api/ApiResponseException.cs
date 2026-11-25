using System;

namespace PatchKit.Api;

public class ApiResponseException : Exception
{
	public int StatusCode { get; }

	public ApiResponseException(int statusCode)
		: base($"API server returned invalid status code {statusCode}")
	{
		StatusCode = statusCode;
	}
}
