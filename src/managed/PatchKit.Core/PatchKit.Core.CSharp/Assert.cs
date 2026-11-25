using System.Linq;

namespace PatchKit.Core.CSharp;

public static class Assert
{
	public static void IsNotNull<T>(T v)
	{
		if (v == null)
		{
			throw new UnpredictedCondition("Value is null.");
		}
	}

	public static void IsNull<T>(T v)
	{
		if (v != null)
		{
			throw new UnpredictedCondition("Value is not null.");
		}
	}

	public static void IsNotNullFromDelegate<T>(T v)
	{
		IsNotNull(v);
	}

	public static void IsValid<T>(T v) where T : struct, IValidatedStruct
	{
		if (!v.IsValid)
		{
			throw new UnpredictedCondition("Value is not valid.");
		}
	}

	public static void IsValid<T>(T? v) where T : struct, IValidatedStruct
	{
		if (v.HasValue && !v.Value.IsValid)
		{
			throw new UnpredictedCondition("Value is not valid.");
		}
	}

	public static void IsValid<T>(ImmutableArray<T> v) where T : struct, IValidatedStruct
	{
		if (v.Any((T x) => !x.IsValid))
		{
			throw new UnpredictedCondition("Value is not valid.");
		}
	}

	public static void IsValid<T>(ImmutableArray<T?> v) where T : struct, IValidatedStruct
	{
		if (v.Any((T? x) => x.HasValue && !x.Value.IsValid))
		{
			throw new UnpredictedCondition("Value is not valid.");
		}
	}

	public static void IsTrue(bool v, string m)
	{
		if (!v)
		{
			throw new UnpredictedCondition(m + " == false");
		}
	}

	public static void IsFalse(bool v, string m)
	{
		if (v)
		{
			throw new UnpredictedCondition(m + " == true");
		}
	}
}
