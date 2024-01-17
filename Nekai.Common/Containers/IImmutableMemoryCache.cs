using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

/// <summary>
/// Defines any class that preloads data for later retrieval.
/// </summary>
/// <typeparam name="TKey"> The type of the key used to retrieve the stored data. </typeparam>
/// <typeparam name="TEntry"> The type of the stored data. </typeparam>
public interface IImmutableMemoryCache<TKey, TEntry> : IDisposable
{
    /// <summary>
    /// Attempt to retrieve an entry from its paired <paramref name="key"/>.
    /// </summary>
    /// <param name="key"> The key paired to the entry to retrieve. </param>
    /// <param name="entry"> The retrieved entry, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the <paramref name="entry"/> has been retrieved, or <see langword="false"/>
    /// otherwise. </returns>
    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TEntry? entry);
    /// <summary>
    /// Retrieve an entry from its paired <paramref name="key"/>.
    /// </summary>
    /// <param name="key"> The key paired to the entry to retrieve. </param>
    /// <returns> The retrieved entry, or <see langword="null"/>. </returns>
    public TEntry? Get(TKey key);
}
