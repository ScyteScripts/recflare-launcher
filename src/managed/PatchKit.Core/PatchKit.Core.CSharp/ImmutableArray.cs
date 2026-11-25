using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PatchKit.Core.CSharp;

public struct ImmutableArray<T> : IEnumerable<T>, IEnumerable, IEquatable<ImmutableArray<T>>
{
	private static readonly IEqualityComparer<T> EqualityComparer = EqualityComparer<T>.Default;

	private readonly T[] _array;

	public static readonly ImmutableArray<T> Empty = default;

	public T this[int index]
	{
		get
		{
			if (_array != null)
			{
				return _array[index];
			}
			throw new IndexOutOfRangeException();
		}
	}

	public int Length { get; }

	public ImmutableArray(IEnumerable<T> source)
	{
		Assert.IsNotNull(source);
		_array = source.ToArray();
		Length = _array.Length;
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (_array == null)
		{
			return Enumerable.Empty<T>().GetEnumerator();
		}
		return ((IEnumerable<T>)_array).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Equals(ImmutableArray<T> other)
	{
		if (Length != other.Length)
		{
			return false;
		}
		if (Length == 0)
		{
			return true;
		}
		Assert.IsNotNull(_array);
		Assert.IsNotNull(other._array);
		return Enumerable.SequenceEqual(_array, other._array);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is ImmutableArray<T> other && Equals(other);
	}

	public override int GetHashCode()
	{
		int num = 17;
		if (_array != null)
		{
			for (int i = 0; i < Length; i++)
			{
				T val = _array[i];
				int num2 = ((val != null) ? EqualityComparer.GetHashCode(val) : 0);
				num = num * 31 + num2;
			}
		}
		return num;
	}

	public static bool operator ==(ImmutableArray<T> left, ImmutableArray<T> right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ImmutableArray<T> left, ImmutableArray<T> right)
	{
		return !left.Equals(right);
	}

	public static ImmutableArray<T> Single(T v)
	{
		return new ImmutableArray<T>(new T[1] { v });
	}

	public override string ToString()
	{
		return $"[Length = {Length}]";
	}
}
