using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nekai.Common;

public class StringSetBuilder<TId, TSelf>
	where TSelf : StringSetBuilder<TId, TSelf>
	where TId : notnull
{
	private ConcurrentDictionary<TId, string> Set { get; } = new();

	public TSelf TryAppend(TId id, string value)
	{
		_ = Set.TryAdd(id, value);
		return (TSelf)this;
	}

	public TSelf Clear()
	{
		Set.Clear();
		return (TSelf)this;
	}

	public virtual string Build()
	{
		return string.Join(' ', Set.Values);
	}
	public override string ToString()
		=> Build();
}