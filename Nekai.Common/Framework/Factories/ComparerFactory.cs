namespace Nekai.Common;

public class ComparerFactory
{
	/// <summary>
	/// Creates an <see cref="IComparer{T}"/> that compares two objects by a single member using its pre-implemented
	/// <see cref="IComparable{T}.CompareTo(T)"/> method.
	/// </summary>
	/// <typeparam name="T"> The generic Type of the <see cref="IComparer{T}"/> to generate. </typeparam>
	/// <typeparam name="TMember"> The Type of the member used for the comparison. </typeparam>
	/// <param name="memberAccessor">The delegate used to retrieve the value used for the comparison.</param>
	/// <returns> An <see cref="IComparer{T}"/> that uses the member retrieved through the <paramref name="memberAccessor"/>
	/// for the comparison. </returns>
	public static IComparer<T> FromMember<T, TMember>(Func<T, TMember> memberAccessor)
		where TMember : IComparable<TMember>
	{
		return Comparer<T>.Create((x, y) => memberAccessor(x).CompareTo(memberAccessor(y)));
	}

	/// <summary>
	/// Creates a comparer that compares two objects by using their pre-implemented
	/// <see cref="IComparable{T}.CompareTo(T)"/> method.
	/// </summary>
	/// <inheritdoc cref="FromMember{T, TMember}(Func{T, TMember})"/>
	public static IComparer<T> Default<T>()
		where T : IComparable<T>
	{
		return Comparer<T>.Create((x, y) => x.CompareTo(y));
	}

	/// <inheritdoc cref="Comparer{T}.Create(Comparison{T})"/>
	public static IComparer<T> Create<T>(Comparison<T> comparison)
		=> Comparer<T>.Create(comparison);
}