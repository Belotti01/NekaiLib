namespace Nekai.Common;

public static class SpanOperationExtensions
{
	public static int CountLines(this ReadOnlySpan<char> span)
	{
		var enumerator = span.GetEnumerator();
		var count = 0;
		while(enumerator.MoveNext())
		{
			count++;
		}
		return count;
	}

	public static ReadOnlySpan<char> SliceLine(this ReadOnlySpan<char> span, int lineIndex)
	{
		if(lineIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(lineIndex), $"Index must be greater or equal to 0.");

		var enumerator = span.EnumerateLines();
		for(int i = 0; i < lineIndex; i++)
		{
			if(!enumerator.MoveNext())
				throw new ArgumentOutOfRangeException(nameof(lineIndex), $"The span contains less than {lineIndex} lines.");
		}
		return enumerator.Current;
	}
}