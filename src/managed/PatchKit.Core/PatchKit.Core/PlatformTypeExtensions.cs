namespace PatchKit.Core;

public static class PlatformTypeExtensions
{
	public static bool IsPosix(this PlatformType @this)
	{
		return @this == PlatformType.Linux64 || @this == PlatformType.Linux32 || @this == PlatformType.Osx64;
	}
}
