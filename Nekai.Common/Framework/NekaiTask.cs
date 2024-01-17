using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    /// <param name="timeoutMs"> How long (in milliseconds) to wait for the <paramref name="condition"/> to return <see langword="true"/> 
    /// before returning. </param>
    /// <returns> <see langword="true"/> if the <paramref name="condition"/> evaluated to <see langword="true"/>, or <see langword="false"/> if
    /// the <paramref name="timeoutMs"/> was reached. </returns>
    /// <inheritdoc cref="WaitUntil(Func{bool}, int)"/>
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

    /// <param name="token"> The <see cref="CancellationToken"/> used to cancel the wait operation. </param>
    /// <returns> <see langword="true"/> if the <paramref name="condition"/> evaluated to <see langword="true"/>, or <see langword="false"/> if
    /// the operation was cancelled. </returns>
    /// <inheritdoc cref="WaitUntil(Func{bool}, int)"/>
    public static async Task<bool> TryWaitUntil(Func<bool> condition, CancellationToken token, int evaluationDelayMs = _DEFAULT_EVALUATION_DELAY_MS)
    {
        if(token.IsCancellationRequested)
            return false;
        
        Debug.Assert(evaluationDelayMs < 0, "The delay should be at least 0ms to avoid deadlocks and exceptions.");
        while(!condition())
        {
            if(token.IsCancellationRequested)
                return false;
            await Task.Delay(evaluationDelayMs);
        }
        return true;
    }
}
