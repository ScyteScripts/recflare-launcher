using System;
using System.Collections.Generic;
using PatchKit.Core.CSharp;

namespace PatchKit.Api;

public class ApiConnectionException : Exception
{
	public Exception MainServerException { get; }

	public ImmutableArray<Exception> CacheServersExceptions { get; }

	public override string Message
	{
		get
		{
			string message = base.Message;
			return message + "\nMain server exceptions:\n" + MainServerException?.ToString() + "Cache servers exceptions:\n" + ExceptionsToString(CacheServersExceptions);
		}
	}

	public ApiConnectionException(Exception mainServerException, ImmutableArray<Exception> cacheServersExceptions)
		: base("Unable to connect to any of the API servers.")
	{
		MainServerException = mainServerException;
		CacheServersExceptions = cacheServersExceptions;
	}

	private static string ExceptionsToString(IEnumerable<Exception> exceptions)
	{
		string text = string.Empty;
		int num = 1;
		foreach (Exception exception in exceptions)
		{
			text += $"{num}. {exception}\n";
			num++;
		}
		if (num == 1)
		{
			text = "(none)";
		}
		return text;
	}
}
