using System;
using Autofac;
using PatchKit.Core;
using PatchKit.Core.CSharp;

namespace PatchKit.Apps.Serialization;

public class ParseMetaDataFormatVersion
{
	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private MetaDataFormatVersion Execute(Text value)
	{
		if (value == "1.0")
		{
			return new MetaDataFormatVersion(1, 0, 0);
		}
		string[] array = value.Value.Split(new char[1] { '.' });
		if (array.Length != 3)
		{
			throw new MetaDataFormatVersionParseFailure(value);
		}
		if (!int.TryParse(array[0], out var result))
		{
			throw new MetaDataFormatVersionParseFailure(value);
		}
		if (!int.TryParse(array[1], out var result2))
		{
			throw new MetaDataFormatVersionParseFailure(value);
		}
		if (!int.TryParse(array[2], out var result3))
		{
			throw new MetaDataFormatVersionParseFailure(value);
		}
		try
		{
			return new MetaDataFormatVersion(result, result2, result3);
		}
		catch (ValidationFailed inner)
		{
			throw new MetaDataFormatVersionParseFailure(value, inner);
		}
	}

	public ParseMetaDataFormatVersion(LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private MetaDataFormatVersion DelegateExecute(Text value, HandleExceptionAndReturnDelegate<MetaDataFormatVersionParseFailure> onMetaDataFormatVersionParseFailure)
	{
		Arg.IsValid(value, "value");
		LogInfo("Executing ParseMetaDataFormatVersion...");
		LogTrace("value = " + value);
		try
		{
			BeginLogRegion(enabled: true);
			MetaDataFormatVersion metaDataFormatVersion = Execute(value);
			EndLogRegion(enabled: true);
			MetaDataFormatVersion metaDataFormatVersion2 = metaDataFormatVersion;
			LogTrace("result = " + metaDataFormatVersion2.ToString());
			LogInfo("ParseMetaDataFormatVersion finished: success.");
			return metaDataFormatVersion;
		}
		catch (MetaDataFormatVersionParseFailure e)
		{
			EndLogRegion(enabled: true);
			LogInfo("ParseMetaDataFormatVersion finished: MetaDataFormatVersionParseFailure.");
			if (onMetaDataFormatVersionParseFailure == null)
			{
				throw;
			}
			object obj = onMetaDataFormatVersionParseFailure(e);
			if (obj == null)
			{
				return default;
			}
			return (MetaDataFormatVersion)obj;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: true);
			LogInfo("ParseMetaDataFormatVersion has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: true);
			LogError("ParseMetaDataFormatVersion failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: true);
			LogError("ParseMetaDataFormatVersion finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public ParseMetaDataFormatVersionDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new ParseMetaDataFormatVersion(context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
	}

	private void LogTrace(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Trace, exception));
	}

	private void LogInfo(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Info, exception));
	}

	private void LogWarning(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Warning, exception));
	}

	private void LogError(string message, Exception exception = null)
	{
		Log(new LogMessage(message, LogMessageType.Error, exception));
	}
}
