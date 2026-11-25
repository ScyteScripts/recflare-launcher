using System;

namespace PatchKit.Core.CSharp;

public struct LogMessage
{
	public string Message { get; }

	public LogMessageType Type { get; }

	public Exception Exception { get; }

	public LogMessage(string message, LogMessageType type, Exception exception = null)
	{
		Assert.IsNotNull(message);
		Message = message;
		Type = type;
		Exception = exception;
	}
}
