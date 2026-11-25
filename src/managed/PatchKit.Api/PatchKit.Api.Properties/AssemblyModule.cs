using Autofac;
using Autofac.Builder;
using Autofac.Core;
using PatchKit.Core.CSharp;
using PatchKit.Core.Properties;
using PatchKit.Network.Properties;

namespace PatchKit.Api.Properties;

public class AssemblyModule : Module
{
	private readonly PatchKit.Core.Properties.AssemblyModule _coreModule;

	private readonly PatchKit.Network.Properties.AssemblyModule _networkModule;

	public static readonly ApiConnectionSettings DefaultApiConnectionSettings = new ApiConnectionSettings(new ApiConnectionServer("api2.patchkit.net", null, 443, useHttps: true), new ApiConnectionServer[1]
	{
		new ApiConnectionServer("api-cache.patchkit.net", null, 443, useHttps: true)
	}.AsImmutable());

	public static readonly ApiConnectionSettings DefaultKeysApiConnectionSettings = new ApiConnectionSettings(new ApiConnectionServer("keys2.patchkit.net", null, 443, useHttps: true), ImmutableArray<ApiConnectionServer>.Empty);

	private readonly ApiConnectionSettings _apiConnectionSettings;

	private readonly ApiConnectionSettings _keysApiConnectionSettings;

	private readonly string _apiCallerId;

	public AssemblyModule(string apiCallerId, ApiConnectionSettings apiConnectionSettings, ApiConnectionSettings keysApiConnectionSettings, PatchKit.Core.Properties.AssemblyModule coreModule, PatchKit.Network.Properties.AssemblyModule networkModule)
	{
		_apiCallerId = apiCallerId;
		_apiConnectionSettings = apiConnectionSettings;
		_keysApiConnectionSettings = keysApiConnectionSettings;
		_coreModule = coreModule;
		_networkModule = networkModule;
	}

	public AssemblyModule(string apiCallerId, PatchKit.Core.Properties.AssemblyModule coreModule, PatchKit.Network.Properties.AssemblyModule networkModule)
		: this(apiCallerId, DefaultApiConnectionSettings, DefaultKeysApiConnectionSettings, coreModule, networkModule)
	{
	}

	protected override void Load(ContainerBuilder builder)
	{
		base.Load(builder);
		builder.RegisterModule(_coreModule);
		builder.RegisterModule(_networkModule);
		builder.Register((IComponentContext context) => new ApiCache());
		builder.RegisterType<ApiCache>().As<IApiCache>();
		builder.RegisterType<BaseApiConnection>().As<IBaseApiConnection>();
		builder.RegisterGeneratedFactory<BaseApiConnectionFactory>(new TypedService(typeof(IBaseApiConnection)));
		builder.RegisterType<ApiConnection>().WithParameter(new NamedParameter("settings", _apiConnectionSettings)).WithParameter(new NamedParameter("apiCallerId", _apiCallerId))
			.As<IApiConnection>();
		builder.RegisterType<KeysApiConnection>().WithParameter(new NamedParameter("settings", _keysApiConnectionSettings)).WithParameter(new NamedParameter("apiCallerId", _apiCallerId))
			.As<IKeysApiConnection>();
	}

	private void AutoLoad(ContainerBuilder builder)
	{
	}
}
