using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nekai.Common;

/// <summary>
/// Immutable data storage for key-value pairs.
/// </summary>
/// <inheritdoc cref="IImmutableMemoryCache{TKey, TValue}"/>
public sealed class ImmutableMemoryCache<TKey, TValue> : IImmutableMemoryCache<TKey, TValue>
	where TKey : notnull
{
	public static ImmutableMemoryCache<TKey, TValue> Empty => new(ImmutableSortedDictionary<TKey, TValue>.Empty);

	/// <summary>
	/// Internal storage of all key-value pairs.
	/// </summary>
	protected ImmutableSortedDictionary<TKey, TValue> Entries { get; private set; }

	/// <inheritdoc cref="ImmutableSortedDictionary{TKey, TValue}.IsEmpty"/>
	public bool IsEmpty => Entries.IsEmpty;

	/// <inheritdoc cref="ImmutableSortedDictionary{TKey, TValue}.Count"/>
	public int Count => Entries.Count;

	private bool _disposed;

	public ImmutableMemoryCache(ImmutableSortedDictionary<TKey, TValue> entries)
	{
		Entries = entries;
	}

	public ImmutableMemoryCache(IDictionary<TKey, TValue> entries, IComparer<TKey>? keyComparer = null)
		: this(entries.ToImmutableSortedDictionary(keyComparer))
	{
	}

	public ImmutableMemoryCache(IEnumerable<TValue> entries, Func<TValue, TKey> keySelector, IComparer<TKey>? keyComparer = null)
		: this(entries.ToImmutableSortedDictionary(keySelector, x => x, keyComparer))
	{
	}

	/// <inheritdoc />
	[Pure]
	public TValue? Get(TKey key)
	{
		_ThrowIfDisposed();

		if(!Entries.TryGetValue(key, out var entry))
			return default;
		return entry;
	}

	/// <inheritdoc />
	[Pure]
	public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue? entry)
	{
		_ThrowIfDisposed();

		return Entries.TryGetValue(key, out entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void _ThrowIfDisposed()
	{
		if(_disposed)
			throw new ObjectDisposedException(nameof(ImmutableMemoryCache<TKey, TValue>));
	}

	public void Dispose()
	{
		if(_disposed)
			return;
		_disposed = true;

		Entries.Clear();
		Entries = null!;
		GC.SuppressFinalize(this);
	}
}