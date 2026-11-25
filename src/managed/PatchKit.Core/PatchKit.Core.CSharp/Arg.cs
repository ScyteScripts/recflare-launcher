using System.Linq;

namespace PatchKit.Core.CSharp;

public static class Arg
{
	public static void IsValid<T>(T v, string n) where T : struct, IValidatedStruct
	{
		if (!v.IsValid)
		{
			throw new ArgNotValid(n);
		}
	}

	public static void IsValid<T>(T? v, string n) where T : struct, IValidatedStruct
	{
		if (v.HasValue && !v.Value.IsValid)
		{
			throw new ArgNotValid(n);
		}
	}

	public static void IsValid<T>(ImmutableArray<T> v, string n) where T : struct, IValidatedStruct
	{
		if (v.Any((T x) => !x.IsValid))
		{
			throw new ArgNotValid(n);
		}
	}

	public static void IsValid<T>(ImmutableArray<T?> v, string n) where T : struct, IValidatedStruct
	{
		if (v.Any((T? x) => x.HasValue && !x.Value.IsValid))
		{
			throw new ArgNotValid(n);
		}
	}

	public static void IsNotNull<T>(T v, string n) where T : class
	{
		if (v == null)
		{
			throw new ArgCantBeNull(n);
		}
	}
}
