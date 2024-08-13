using System.Numerics;

namespace Nekai.Common;

/// <summary>
/// Superset of <see cref="HashSet{T}"/> that implements set operation syntax.
/// </summary>
/// <inheritdoc cref="HashSet{T}"/>
public sealed class Set<T> : HashSet<T>, IEquatable<IEnumerable<T>>,
	// Properly implements operator interfaces
	IEqualityOperators<Set<T>, IEnumerable<T>, bool>,
	IAdditionOperators<Set<T>, T, Set<T>>,
	ISubtractionOperators<Set<T>, T, Set<T>>,
	IAdditionOperators<Set<T>, IEnumerable<T>, Set<T>>,
	ISubtractionOperators<Set<T>, IEnumerable<T>, Set<T>>,
	IModulusOperators<Set<T>, IEnumerable<T>, Set<T>>,
	IComparisonOperators<Set<T>, IEnumerable<T>, bool>
// No idea how to implement the ~ operator (all possible values except those in the Set), so let's leave it at this for now
{
	// Null values are treated as empty sets for comparisons
	public static bool operator <(Set<T>? a, IEnumerable<T>? b) => b is not null && (a is null || a.IsProperSubsetOf(b));

	public static bool operator >(Set<T>? a, IEnumerable<T>? b) => a is not null && (b is null || a.IsProperSupersetOf(b));

	public static bool operator <=(Set<T>? a, IEnumerable<T>? b) => a is null || (b is not null && a.IsSubsetOf(b));

	public static bool operator >=(Set<T>? a, IEnumerable<T>? b) => b is null || (a is not null && a.IsSupersetOf(b));

	public static bool operator ==(Set<T>? a, IEnumerable<T>? b) => a is null ? b is null : b is not null && a.SetEquals(b);

	public static bool operator !=(Set<T>? a, IEnumerable<T>? b) => !(a == b);

	public static Set<T> operator |(Set<T> a, IEnumerable<T> b)
	{
		Set<T> set = [];
		set.UnionWith(a);
		set.UnionWith(b);
		return set;
	}

	public static Set<T> operator &(Set<T> a, IEnumerable<T> b)
	{
		Set<T> set = [];
		set.UnionWith(a);
		set.IntersectWith(b);
		return set;
	}

	public static Set<T> operator ^(Set<T> a, IEnumerable<T> b)
	{
		Set<T> set = [];
		set.UnionWith(a);
		set.SymmetricExceptWith(b);
		return set;
	}

	public static Set<T> operator +(Set<T> a, T b)
	{
		Set<T> set = [];
		set.UnionWith(a);
		set.Add(b);
		return set;
	}

	public static Set<T> operator -(Set<T> a, T b)
	{
		Set<T> set = [];
		set.UnionWith(a);
		set.Remove(b);
		return set;
	}

	public static Set<T> operator +(Set<T> a, IEnumerable<T> b)
	{
		Set<T> set = [];
		set.UnionWith(a);
		foreach(T value in b)
		{
			a.Add(value);
		}
		return set;
	}

	public static Set<T> operator %(Set<T> a, IEnumerable<T> b) => a - b;

	public static Set<T> operator -(Set<T> a, IEnumerable<T> b)
	{
		Set<T> set = [];
		set.UnionWith(a);
		set.ExceptWith(b);
		return set;
	}

	/// <summary>
	/// Generate a <see langword="string"/> displaying the elements of this <see cref="Set{T}"/> in
	/// Set Notation.
	/// </summary>
	public override string ToString()
	{
		return $"{{ {string.Join(", ", this)} }}";
	}

	public override int GetHashCode() => base.GetHashCode();

	public override bool Equals(object? obj) => base.Equals(obj);

	public bool Equals(IEnumerable<T>? other)
		=> other is not null
		&& this == other;
}