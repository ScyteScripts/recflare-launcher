using Autofac;
using PatchKit.Core.Properties;

namespace PatchKit.Network.Properties;

public class AssemblyModule : Module
{
	private readonly PatchKit.Core.Properties.AssemblyModule _coreModule;

	public AssemblyModule(PatchKit.Core.Properties.AssemblyModule coreModule)
	{
		_coreModule = coreModule;
	}

	protected override void Load(ContainerBuilder builder)
	{
		base.Load(builder);
		builder.RegisterModule(_coreModule);
		AutoLoad(builder);
	}

	private void AutoLoad(ContainerBuilder builder)
	{
		GetBufferedHttpStreamedResponse.RegisterInContainer(builder);
		GetHttpResponse.RegisterInContainer(builder);
		GetHttpStreamedResponse.RegisterInContainer(builder);
		GetHttpTextResponse.RegisterInContainer(builder);
	}
}
