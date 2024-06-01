namespace Nekai.Common;

public class ReadOnlyExecutionProgress
{
	/// <summary>
	/// The time at which the operation started.
	/// </summary>
	public DateTime UTCStartTime { get; protected set; } = DateTime.UtcNow;

	// Used to calculate the ElapsedTime once the operation is completed.
	private DateTime? _completionTimeUTC = null;

	private float _progress = 0;

	/// <summary>
	/// Percentage of the operation that has been completed. Value will always be in the range 0 to 100 (inclusive).
	/// </summary>
	public float Progress
	{
		get => _progress;
		protected set
		{
			_progress = float.Clamp(value, 0, 100);
			if(_progress == 100)
			{
				// Only set the completion time once.
				_completionTimeUTC ??= DateTime.UtcNow;
			}
		}
	}

	/// <summary>
	/// The last status message set by the operation.
	/// </summary>
	public string? StatusMessage { get; protected set; }

	/// <summary>
	/// The time elapsed since the operation started.
	/// </summary>
	public TimeSpan ElapsedTime => (_completionTimeUTC ?? DateTime.UtcNow) - UTCStartTime;

	/// <summary>
	/// The estimated time remaining for the operation to complete, or <see cref="TimeSpan.Zero"/> if no progress has been done.
	/// </summary>
	public TimeSpan ETA => Progress is 0 or 100
 		? TimeSpan.Zero
		: (ElapsedTime / Progress) * (100 - Progress);

	/// <inheritdoc cref="ToString()"/>
	/// <param name="progressBarSteps"> The amount of steps displayed in the progress bar. </param>
	public string ToString(int progressBarSteps)
	{
		const string TIMESPAN_FORMAT = @"hh\:mm\:ss";
		string elapsedString = $"Elapsed: {ElapsedTime.ToString(TIMESPAN_FORMAT)}";
		string etaString = $"ETA: {ETA.ToString(TIMESPAN_FORMAT)}";
		string progressBar = NekaiString.CreateProgressBar((int)Progress, progressBarSteps);

		string str = StatusMessage is null
			? ""
			: StatusMessage + '\n';

		str = $"{str}{elapsedString}\n{Progress}% {progressBar}\n{etaString}";
		return str;
	}

	/// <summary>
	/// Returns a string representation of the progress.
	/// </summary>
	/// <returns>A <see langword="string"/> containing the <see cref="StatusMessage"/>, the <see cref="ElapsedTime"/>,
	/// the <see cref="Progress"/> followed by a progress bar, and the <see cref="ETA"/>, all split by a newline character.</returns>
	public override string ToString()
		=> ToString(10);
}