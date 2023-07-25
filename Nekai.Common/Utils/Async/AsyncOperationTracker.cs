﻿namespace Nekai.Common;

// Use a static Factory implementation to ensure proper initialization while maintaining correct encapsulation.
// This is necessary because of the two-way injection of the ExecutionProgress when using a Task generation Function, which 
// requires the ExecutionProgress to be initialized before the Task is created.

/// <summary>
/// Wrapper class for <see cref="Task"/> that allows tracking the progress of the <see cref="Task"/>.
/// </summary>
public sealed class AsyncOperationTracker
{
	/// <summary> Downcasting of <see cref="Progress"/> that enables write operations. </summary>
	private ExecutionProgress _EditableProgress => (ExecutionProgress)Progress;

	/// <summary> The execution progress of the tracked operation. </summary>
	public ReadOnlyExecutionProgress Progress { get; }
	/// <summary> Whether the tracked operation has completed its execution. </summary>
	public bool ExecutionEnded { get; private set; } = false;
	/// <summary> The <see cref="TaskStatus"/> of the tracked operation. </summary>
	public TaskStatus Status => _operation.Status;
	
	private readonly Task _operation;



	private AsyncOperationTracker(Task operation, ReadOnlyExecutionProgress progressTracker)
	{
		Progress = progressTracker;
		_operation = operation;
	}

	private AsyncOperationTracker(Action<ExecutionProgress> operation)
	{
		Progress = new();
		_operation = new(() => operation((ExecutionProgress)Progress));
	}



	/// <summary>
	/// Generate an instance of <see cref="AsyncOperationTracker"/> by injecting a progress tracking object into a <see cref="Task"/>.
	/// </summary>
	/// <param name="taskCreationOperation"> 
	/// The operation that creates the <see cref="Task"/> to track using the injected progress tracking parameter.
	/// </param>
	/// <returns> An <see cref="AsyncOperationTracker"/> that tracks the <see cref="Task"/> generated by the 
	/// <paramref name="taskCreationOperation"/>. </returns>
	/// <remarks> Make sure that the <paramref name="taskCreationOperation"/> does not return an already started <see cref="Task"/>. </remarks>
	/// <exception cref="Exception"> Thrown when the <paramref name="taskCreationOperation"/> throws an Exception (see the <see cref="Exception.InnerException"/>). </exception>
	/// <exception cref="InvalidOperationException"> Thrown when the operation has already previously started execution. </exception>
	public static AsyncOperationTracker Create(Func<ExecutionProgress, Task> taskCreationOperation)
	{
		ExecutionProgress progress = new();
		Task operation;

		try
		{
			operation = taskCreationOperation.Invoke(progress);
		}
		catch(Exception ex)
		{
			throw new Exception("Task creation operation failed. See inner exception for more information.", ex);
		}

		if(operation.WasActivated())
			// Operation started prematurely
			throw new InvalidOperationException("The generated operation has already started executing.");

		AsyncOperationTracker tracker = new(operation, progress);
		return tracker;
	}

	/// <summary>
	/// Generate a new instance of <see cref="AsyncOperationTracker"/> by injecting a progress tracking object into an <see cref="Action"/>.
	/// </summary>
	/// <param name="operation"> The operation to execute and track. </param>
	/// <returns> A new <see cref="AsyncOperationTracker"/> wrapping the <paramref name="operation"/> as a <see cref="Task"/>. </returns>
	public static AsyncOperationTracker Create(Action<ExecutionProgress> operation)
	{
		return new(operation);
	}

	/// <summary>
	/// Begin the execution of the tracked operation on a background thread, and set the start time of the operation.
	/// </summary>
	public void Start()
	{
		// Append the OnOperationEnded callback once the operation completes.
		_operation.ContinueWith(_OnOperationEnded);
		_EditableProgress.ResetStartTime();
		_operation.Start();
	}

	/// <summary>
	/// Run the tracked operation to completion asynchronously, after setting the start time of the operation.
	/// </summary>
	/// <param name="token"> The <see cref="CancellationToken"/> to use to cancel the operation. </param>
	public async Task RunAsync(CancellationToken token = default)
	{
		if(token.IsCancellationRequested)
			return;

		await _operation.ContinueWith(_OnOperationEnded, token);
		_EditableProgress.ResetStartTime();
		await _operation;
	}

	/// <summary>
	/// Synchronously wait for the tracked operation to complete.
	/// </summary>
	/// <inheritdoc cref="Task.Wait()"/>
	public void WaitForCompletion()
	{
		if(ExecutionEnded)
			return;

		_operation.Wait();
	}

	private void _OnOperationEnded(Task operation)
	{
		ExecutionEnded = true;
		// Update the Progress to the correct state to avoid deadlocks.
		_EditableProgress.Progress = 100f;
	}
}
