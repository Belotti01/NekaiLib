﻿using System.Diagnostics;

namespace Nekai.Common;

/// <summary>
/// <typeparamref name="TValue"/> values generator with tests implemented that run in DEBUG mode to ensure that only unique
/// values are generated by each instance.
/// </summary>
/// <typeparam name="TValue"> The type of the values to be generated. Must implement IEquatable (or be a <see langword="record"/> type). </typeparam>
public class UniqueValueGenerator<TValue>
	where TValue : IEquatable<TValue>
{
	private const int _TEST_ITERATIONS = 500;

	/// <summary>
	/// Functions defining the generation of each ID, based on the last generated value (taken as parameter)
	/// </summary>
	protected readonly Func<TValue?, TValue> _generator;

	private TValue? _lastValue;

	/// <summary>
	/// Create a new ID generator based on the <paramref name="generator"/> function, which takes the
	/// last generated ID as parameter to return a new unique value.
	/// </summary>
	/// <param name="generator">The ID generation function.</param>
	/// <param name="startIdValue">The first value sent to the <paramref name="generator"/> upon invoking the
	/// <see cref="Next"/> method.</param>
	public UniqueValueGenerator(Func<TValue?, TValue> generator, TValue? startIdValue = default)
	{
		_generator = generator;
		_lastValue = startIdValue;
		_TestIdGenerator(generator, startIdValue);
	}

	public UniqueValueGenerator(Func<TValue> generator, TValue? startIdValue = default)
		: this(x => generator(), startIdValue)
	{
	}

	/// <summary>
	/// Generate a new ID but don't update the state of the generator.
	/// </summary>
	/// <returns>An ID value that can be generated at the next call of <see cref="Next"/>.</returns>
	public TValue Peek() => _generator(_lastValue);

	/// <summary>
	/// Generate a new ID.
	/// </summary>
	public TValue Next()
	{
		lock(this)
		{
			_lastValue = _generator(_lastValue);
			return _lastValue;
		}
	}

	/// <summary>
	/// In DEBUG mode, runs the <paramref name="generator"/> for <see cref="_TEST_ITERATIONS"/> times, and asserts that none of the values
	/// is a duplicate of another.
	/// </summary>
	/// <param name="generator"> The generator function to test. </param>
	/// <param name="previousValue"> A value that has already been generated using the <paramref name="generator"/>, or 
	/// <see langword="null"/>. </param>
	[Conditional("DEBUG")]
	private static void _TestIdGenerator(Func<TValue?, TValue> generator, TValue? previousValue = default)
	{
		HashSet<TValue> ids = new(_TEST_ITERATIONS);

		if(previousValue is not null)
		{
			ids.Add(previousValue);
		}

		for(int i = 0; i < _TEST_ITERATIONS; ++i)
		{
			previousValue = generator(previousValue);
			Debug.Assert(previousValue is not null, $"{nameof(UniqueValueGenerator<TValue>)} generated a null value.");
			Debug.Assert(ids.Add(previousValue), $"{nameof(UniqueValueGenerator<TValue>)} does not consistently generate unique ids.");
		}
	}
}