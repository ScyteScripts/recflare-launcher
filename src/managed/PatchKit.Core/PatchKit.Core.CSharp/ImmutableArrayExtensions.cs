using System.Collections.Generic;

namespace PatchKit.Core.CSharp;

public static class ImmutableArrayExtensions
{
	public static ImmutableArray<T> AsImmutable<T>(this IEnumerable<T> @this)
	{
		Assert.IsNotNull(@this);
		return new ImmutableArray<T>(@this);
	}
}
