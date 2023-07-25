namespace Nekai.Common;

public sealed class ExecutionProgress : ReadOnlyExecutionProgress
{
	// Adds a public setter to the ReadOnlyExecutionProgress properties.

	/// <inheritdoc cref="ReadOnlyExecutionProgress.Progress"/>
	public new float Progress
	{
		get => base.Progress;
		set => base.Progress = value;
	}

	/// <inheritdoc cref="ReadOnlyExecutionProgress.StatusMessage"/>
	public new string? StatusMessage
	{
		get => base.StatusMessage;
		set => base.StatusMessage = value;
	}

	/// <summary> Overwrite the <see cref="ReadOnlyExecutionProgress.UTCStartTime"/> with the current date and time. </summary>
	public void ResetStartTime()
	{
		UTCStartTime = DateTime.UtcNow;
	}
}
