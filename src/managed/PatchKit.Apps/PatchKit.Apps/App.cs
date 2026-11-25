using PatchKit.Core.IO;

namespace PatchKit.Apps;

public class App
{
	public Path Path { get; }

	public AppMetaData? MetaDataCache { get; set; }

	public App(Path path)
	{
		Path = path;
	}
}
