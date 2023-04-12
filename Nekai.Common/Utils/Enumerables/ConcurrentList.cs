using System.Collections;

namespace Nekai.Common;

// UNOPTIMIZED
// TODO: For each operation, determine which methods would result in thread-unsafe behavior when run concurrently,
// and integrate more specific locks to only prevent those (instead of locking all possible operations for each).

/// <summary>
/// Wrapper of a <see cref="List{T}"/> that locks access to achieve thread-safety.
/// </summary>
/// <typeparam name="T">The Type of the elements of the list.</typeparam>
public class ConcurrentList<T> : IList<T>, IReadOnlyList<T>
{
	private readonly object _lock = new();
	private readonly List<T> _internalList;

	/// <inheritdoc/>
	public int Count => _internalList.Count;

	/// <inheritdoc/>
	public bool IsReadOnly => false;

	/// <inheritdoc/>
	public IEnumerator<T> Enumerator => GetEnumerator();

	/// <inheritdoc/>
	public T this[int index]
	{
		get
		{
			lock(_lock)
				return _internalList[index];
		}
		set
		{
			lock(_lock)
				_internalList[index] = value;
		}
	}

	/// <inheritdoc cref="List{T}.List()"/>
	public ConcurrentList()
	{
		_internalList = new();
	}

	/// <inheritdoc cref="List{T}.List(IEnumerable{T})"/>
	public ConcurrentList(IEnumerable<T> collection)
	{
		_internalList = new(collection);
	}

	/// <inheritdoc cref="List{T}.List(int)"/>
	public ConcurrentList(int capacity)
	{
		_internalList = new(capacity);
	}

	/// <inheritdoc/>
	public int IndexOf(T item)
	{
		lock(_lock)
			return ((IList<T>)_internalList).IndexOf(item);
	}

	/// <inheritdoc/>
	public void Insert(int index, T item)
	{
		lock(_lock)
			((IList<T>)_internalList).Insert(index, item);
	}

	/// <inheritdoc/>
	public void RemoveAt(int index)
	{
		lock(_lock)
			((IList<T>)_internalList).RemoveAt(index);
	}

	/// <inheritdoc/>
	public void Add(T item)
	{
		lock(_lock)
			((ICollection<T>)_internalList).Add(item);
	}

	/// <inheritdoc/>
	public void Clear()
	{
		lock(_lock)
			((ICollection<T>)_internalList).Clear();
	}

	/// <inheritdoc/>
	public bool Contains(T item)
	{
		lock(_lock)
			return ((ICollection<T>)_internalList).Contains(item);
	}

	/// <inheritdoc/>
	public void CopyTo(T[] array, int arrayIndex)
	{
		lock(_lock)
			((ICollection<T>)_internalList).CopyTo(array, arrayIndex);
	}

	/// <inheritdoc/>
	public bool Remove(T item)
	{
		lock(_lock)
			return ((ICollection<T>)_internalList).Remove(item);
	}

	/// <inheritdoc/>
	public IEnumerator<T> GetEnumerator()
	{
		// Enumerators are immutable so we don't need to re-implement it to achieve thread-safety
		lock(_lock)
			return _internalList.GetEnumerator();
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();
}