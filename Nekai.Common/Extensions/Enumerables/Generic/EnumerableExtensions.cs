using System.Numerics;

namespace Nekai.Common;

public static class EnumerableExtensions
{
	public static (IEnumerable<T>, IEnumerable<T>) Partition<T>(this IEnumerable<T> nums, Predicate<T> condition)
		where T : INumber<T>
	{
		(IEnumerable<T> partition1, IEnumerable<T> partition2) result = new([], []);

		foreach(var item in nums)
		{
			if(condition(item))
				result.partition1 = result.partition1.Append(item);
			else
				result.partition2 = result.partition2.Append(item);
		}

		return result;
	}

	public static string ToString<T>(this IEnumerable<T> elements, string divisor)
		=> string.Join(divisor, elements);

	public static string ToString<T>(this IEnumerable<T> elements, char divisor)
		=> string.Join(divisor, elements);
}