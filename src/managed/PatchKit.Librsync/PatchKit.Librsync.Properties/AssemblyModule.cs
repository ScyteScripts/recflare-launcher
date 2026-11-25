using Autofac;

namespace PatchKit.Librsync.Properties;

public class AssemblyModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		base.Load(builder);
		AutoLoad(builder);
	}

	private void AutoLoad(ContainerBuilder builder)
	{
		PatchFile.RegisterInContainer(builder);
		PatchFileOverwrite.RegisterInContainer(builder);
	}
}
