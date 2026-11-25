using System;
using System.Collections.Generic;
using System.Linq;

namespace PatchKit.Core.CSharp;

public static class EnumerableExtensions
{
	public static T? FirstOrNull<T>(this IEnumerable<T> @this, Func<T, bool> predicate = null) where T : struct
	{
		Assert.IsNotNull(@this);
		if (predicate == null)
		{
			predicate = (T _) => true;
		}
		return @this.Select((Func<T, T?>)((T v) => v)).FirstOrDefault((T? x) => predicate(x.Value));
	}
}
