using PatchKit.Core.CSharp;

namespace PatchKit.Apps;

public class AppMetaDataFilePaths
{
	public static readonly ImmutableArray<string> Deprecated = new ImmutableArray<string>(new string[1] { "patcher_cache.json" });

	public static readonly string Current = "app_data.json";
}
