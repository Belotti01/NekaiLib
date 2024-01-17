using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Nekai.Common;

public class NekaiTimer 
    : Timer
{
    /// <summary>
    /// Indicates whether the <see cref="Timer.Elapsed"/> event has been raised.
    /// </summary>
    public bool TimedOut { get; private set; } = false;

    /// <summary>
    /// Initialize a new instance of <see cref="NekaiTimer"/>, of duration <paramref name="milliseconds"/> if greater than 0, 
    /// or 1ms otherwise.
    /// </summary>
    /// <param name="milliseconds"> The duration of the <see cref="Timer"/>, in milliseconds. </param>
    public NekaiTimer(double milliseconds)
        // Prevent exceptions.
        : base(milliseconds <= 0 ? 1 : milliseconds)
    {
        // Do not set TimedOut to true even if milliseconds is <= 0; wait for the timer to be started.
        Elapsed += _TimeOut;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="NekaiTimer"/>, of duration <paramref name="timeout"/>.
    /// </summary>
    /// <param name="timeout"> The duration of the <see cref="Timer"/>. </param>
    public NekaiTimer(TimeSpan timeout)
        : this(timeout.TotalMilliseconds)
    {
    }

    /// <summary>
    /// Initialize and start a new instance of <see cref="NekaiTimer"/>, of duration <paramref name="milliseconds"/> if greater than 0,
    /// or 1ms otherwise.
    /// </summary>
    /// <returns> A new instance of <see cref="NekaiTimer"/> that has already been enabled. </returns>
    /// <inheritdoc cref="NekaiTimer(double)"/>
    public static NekaiTimer Start(double milliseconds)
    {
        NekaiTimer timer = new(milliseconds);
        timer.Start();
        return timer;
    }

    /// <summary>
    /// Initialize and start a new instance of <see cref="NekaiTimer"/>, of duration <paramref name="timeout"/>.
    /// </summary>
    /// <returns> A new instance of <see cref="NekaiTimer"/> that has already been enabled. </returns>
    /// <inheritdoc cref="NekaiTimer(TimeSpan)"/>
    public static NekaiTimer Start(TimeSpan timeout)
        => Start(timeout.TotalMilliseconds);

    private void _TimeOut(object? sender, ElapsedEventArgs args)
    {
        TimedOut = true;
    }
}
