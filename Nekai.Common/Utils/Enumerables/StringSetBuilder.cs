﻿namespace Nekai.Common;

/// <summary>
/// Thread-locking StringBuilder specialized for CSS classes. Avoids duplicate strings and
/// avoids double values for classes of the same type with <see cref="AppendIfUnique"/>.
/// "Append" methods are made to return <see langword="this"/> for more readable usage.
/// </summary>
public class StringSetBuilder
{
	protected ISet<string> Set { get; } = new HashSet<string>();

	/// <summary>
	/// Appends a string to the result if it is not already present.
	/// </summary>
	/// <param name="value">The string to append.</param>
	public StringSetBuilder Append(string? value)
	{
		if(string.IsNullOrEmpty(value))
			return this;

		lock(this)
		{
			Set.Add(value);
		}
		return this;
	}

	/// <summary>
	/// Appends multiple strings to the result, each only if it is not already present.
	/// </summary>
	/// <param name="values">The strings to append.</param>
	public StringSetBuilder Append(params string?[] values)
	{
		lock(this)
		{
			string? value;
			for(int i = values.Length - 1; i >= 0; --i)
			{
				value = values[i];
				if(!string.IsNullOrEmpty(value))
				{
					Set.Add(value);
				}
			}
		}
		return this;
	}

	/// <summary>
	/// Only append the <paramref name="value"/> string to the result if none of the previously entered fields
	/// starts with the specified <paramref name="uniqueStart"/>.
	/// </summary>
	/// <param name="value">The string to append.</param>
	/// <param name="uniqueStart">The beginning part of the <paramref name="value"/>, which identifies its domain.</param>
	public StringSetBuilder AppendIfUnique(string value, ReadOnlySpan<char> uniqueStart)
	{
		lock(this)
		{
			foreach(ReadOnlySpan<char> current in Set)
			{
				if(current.StartsWith(uniqueStart))
					return this;
			}

			Set.Add(value);
		}

		return this;
	}

	/// <summary>
	/// Delete all characters from the result string.
	/// </summary>
	public void Clear()
	{
		lock(this)
		{
			Set.Clear();
		}
	}

	public string Build()
	{
		string result;
		lock(this)
		{
			result = string.Join(" ", Set);
		}
		return result;
	}

	public override string ToString()
		=> Build();
}