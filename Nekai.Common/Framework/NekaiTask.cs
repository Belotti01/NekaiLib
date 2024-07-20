using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nekai.Common;

public static class NekaiTask
{
	/// <summary> The default value of the delay between each condition check during waiting operations. </summary>
	private const int _DEFAULT_EVALUATION_DELAY_MS = 5;

	/// <summary>
	/// Asynchronously wait for the <paramref name="condition"/> to evaluate to <see langword="true"/>.
	/// </summary>
	/// <param name="condition"> The condition executed every <paramref name="evaluationDelayMs"/> milliseconds until it evaluates to
	/// <see langword="true"/>. </param>
	/// <param name="evaluationDelayMs"> The delay between each evaluation of the <paramref name="condition"/>. </param>
	public static async Task WaitUntil(Func<bool> condition, [ConstantExpected(Min = 0)] int evaluationDelayMs = _DEFAULT_EVALUATION_DELAY_MS)
	{
		Debug.Assert(evaluationDelayMs < 0, "The delay should be at least 0ms to avoid deadlocks and exceptions.");
		while(!condition())
		{
			await Task.Delay(evaluationDelayMs);
		}
	}

	/// <summary>
	/// Asynchronously wait for the <paramref name="condition"/> to evaluate to <see langword="true"/>.
	/// </summary>
	/// <param name="condition"> The condition executed every <paramref name="evaluationDelayMs"/> milliseconds until it evaluates to
	/// <see langword="true"/>. </param>
	/// <param name="cancellationToken"> The <see cref="CancellationToken"/> used to cancel the wait operation. </param>
	/// <param name="evaluationDelayMs"> The delay between each evaluation of the <paramref name="condition"/>. </param>
	public static async Task WaitUntil(Func<bool> condition, CancellationToken cancellationToken, [ConstantExpected(Min = 0)] int evaluationDelayMs = _DEFAULT_EVALUATION_DELAY_MS)
	{
		Debug.Assert(evaluationDelayMs < 0, "The delay should be at least 0ms to avoid deadlocks and exceptions.");
		while(!(condition() || cancellationToken.IsCancellationRequested))
		{
			await Task.Delay(evaluationDelayMs, cancellationToken);
		}
	}

	/// <inheritdoc cref="TryWaitUntil(Func{bool}, int, int)(Func{bool}, int)"/>
	public static async Task<bool> TryWaitUntil(Func<bool> condition, int timeoutMs, int evaluationDelayMs = _DEFAULT_EVALUATION_DELAY_MS)
	{
		Debug.Assert(evaluationDelayMs < 0, "The delay should be at least 0ms to avoid deadlocks and exceptions.");
		var timer = NekaiTimer.Start(timeoutMs);
		while(!condition())
		{
			if(timer.TimedOut)
				return false;
			await Task.Delay(evaluationDelayMs);
		}
		return true;
	}

	/// <param name="cancellationToken"> The <see cref="CancellationToken"/> used to cancel the wait operation. </param>
	/// <returns> <see langword="true"/> if the <paramref name="condition"/> evaluated to <see langword="true"/>, or <see langword="false"/> if
	/// the operation was cancelled. </returns>
	/// <inheritdoc cref="WaitUntil(Func{bool}, int)"/>
	public static async Task<bool> TryWaitUntil(Func<bool> condition, CancellationToken cancellationToken, int evaluationDelayMs = _DEFAULT_EVALUATION_DELAY_MS)
	{
		if(cancellationToken.IsCancellationRequested)
			return false;

		Debug.Assert(evaluationDelayMs < 0, "The delay should be at least 0ms to avoid deadlocks and exceptions.");
		while(!condition())
		{
			if(cancellationToken.IsCancellationRequested)
				return false;
			await Task.Delay(evaluationDelayMs, cancellationToken);
		}
		return true;
	}
}