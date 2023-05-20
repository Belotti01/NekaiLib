namespace Nekai.Common;

public static class TaskExtensions
{
	// More comprehensive checks for the status of a Task.

	/// <summary>
	/// Check whether the <see cref="Task"/> has at any point been activated.
	/// </summary>
	/// <param name="task"> The task to check. </param>
	/// <returns> <see langword="true"/> if the <see cref="Task"/>'s execution has been previously requested; 
	/// <see langword="false"/> otherwise. </returns>
	public static bool WasActivated(this Task task)
	{
		return task.Status > TaskStatus.WaitingForActivation;
	}

	/// <summary>
	/// Check whether the <see cref="Task"/> can be invoked.
	/// </summary>
	/// <param name="task"> The task to check. </param>
	/// <returns> <see langword="true"/> if the <see cref="Task"/> has yet to be invoked; <see langword="false"/> otherwise. </returns>
	public static bool CanBeInvoked(this Task task)
	{
		return task.Status <= TaskStatus.WaitingForActivation;
	}

	/// <summary>
	/// Check whether the <see cref="Task"/> has thrown an Exception during its execution.
	/// </summary>
	/// <param name="task"> The task to check. </param>
	/// <returns> <see langword="true"/> if an error occurred during the execution of the <see cref="Task"/>; <see langword="false"/>
	/// otherwise. </returns>
	public static bool Errored(this Task task)
	{
		return task.Status == TaskStatus.Faulted;
	}
}
