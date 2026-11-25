using System;
using Autofac;
using Newtonsoft.Json;
using PatchKit.Core;
using PatchKit.Core.CSharp;
using PatchKit.Network;

namespace PatchKit.Apps.Remote;

public class GetCountryCode
{
	private struct Info
	{
		[JsonProperty("country")]
		public string Country;
	}

	public GetHttpTextResponseDelegate GetHttpTextResponse { get; }

	public LogDelegate Log { get; }

	public BeginLogRegionDelegate BeginLogRegion { get; }

	public EndLogRegionDelegate EndLogRegion { get; }

	private Text? Execute()
	{
		HttpRequestInfo requestInfo = new HttpRequestInfo(new HttpAddress(new Uri("https://ip2loc.patchkit.net/v1/country")), HttpRequestMethod.Get, null, null, ImmutableArray<HttpRequestHeader>.Empty);
		HttpTextResponse httpTextResponse;
		try
		{
			httpTextResponse = GetHttpTextResponse(requestInfo, new Timeout(TimeSpan.FromSeconds(10.0)), null);
		}
		catch (ConnectionFailure exception)
		{
			LogWarning("Unable to get country code due to connection failure.", exception);
			return null;
		}
		Info info;
		try
		{
			info = JsonConvert.DeserializeObject<Info>(httpTextResponse.Body);
		}
		catch (JsonSerializationException exception2)
		{
			LogWarning("Unable to deserialize country code.", exception2);
			return null;
		}
		if (info.Country == null)
		{
			LogWarning("Country code is null.");
			return null;
		}
		return new Text(info.Country);
	}

	public GetCountryCode(GetHttpTextResponseDelegate getHttpTextResponse, LogDelegate log, BeginLogRegionDelegate beginLogRegion, EndLogRegionDelegate endLogRegion)
	{
		Assert.IsNotNull(getHttpTextResponse);
		Assert.IsNotNull(log);
		Assert.IsNotNull(beginLogRegion);
		Assert.IsNotNull(endLogRegion);
		GetHttpTextResponse = getHttpTextResponse;
		Log = log;
		BeginLogRegion = beginLogRegion;
		EndLogRegion = endLogRegion;
	}

	private Text? DelegateExecute()
	{
		LogInfo("Executing GetCountryCode...");
		try
		{
			BeginLogRegion(enabled: false);
			Text? text = Execute();
			EndLogRegion(enabled: false);
			Text? text2 = text;
			LogTrace("result = " + (text2.HasValue ? ((string)text2.GetValueOrDefault()) : null));
			LogInfo("GetCountryCode finished: success.");
			return text;
		}
		catch (OperationCanceledException)
		{
			EndLogRegion(enabled: false);
			LogInfo("GetCountryCode has been cancelled.");
			throw;
		}
		catch (FunctionFatalError)
		{
			EndLogRegion(enabled: false);
			LogError("GetCountryCode failed due to other function failure.");
			throw;
		}
		catch (Exception ex2)
		{
			EndLogRegion(enabled: false);
			LogError("GetCountryCode finished with fatal error.", ex2);
			throw new FunctionFatalError(ex2);
		}
	}

	public GetCountryCodeDelegate GetDelegate()
	{
		return DelegateExecute;
	}

	public static void RegisterInContainer(ContainerBuilder builder)
	{
		builder.Register((IComponentContext context) => new GetCountryCode(context.Resolve<GetHttpTextResponseDelegate>(), context.Resolve<LogDelegate>(), context.Resolve<BeginLogRegionDelegate>(), context.Resolve<EndLogRegionDelegate>()).GetDelegate());
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
