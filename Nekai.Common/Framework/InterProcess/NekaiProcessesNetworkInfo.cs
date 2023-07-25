using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace Nekai.Common;

public sealed class NekaiProcessesNetworkInfo : ConfigurationFileManager<NekaiProcessesNetworkInfo>, IDisposable
{
	[JsonIgnore]
	private const int _SERIALIZATION_MAX_ATTEMPTS = 10;
	[JsonIgnore]
	private const int _SERIALIZATION_ATTEMPT_DELAY_MS = 50;

	[JsonInclude]
	public NekaiProcessNetworkInfo[] Processes { get; set; } = Array.Empty<NekaiProcessNetworkInfo>();
	[JsonIgnore]
	private CancellationTokenSource _serializationCancellationTokenSource = new();
	[JsonIgnore]
	private Task? _serializationTask;

	public NekaiProcessesNetworkInfo(string filePath)
		: base(filePath) { }

	public async Task UpdateCurrentProcessInfoAsync(ProtocolType protocol, int port)
	{
		bool requiresUpdate = _UpdateCurrentProcessInfoInternal(protocol, port);
		if(!requiresUpdate)
			return;

		await _SerializeWithMultipleAttempts(_serializationCancellationTokenSource.Token);
	}

	public void UpdateCurrentProcessInfo(ProtocolType protocol, int port)
	{
		// Avoid instantiating and managing a new Task if it's not necessary.
		bool requiresUpdate = _UpdateCurrentProcessInfoInternal(protocol, port);
		if(!requiresUpdate)
			return;

		if(_serializationTask is not null)
		{
			// Need to cancel previously started tasks
			if(!_serializationCancellationTokenSource.IsCancellationRequested)
			{
				_serializationCancellationTokenSource.Cancel();
				_serializationTask.Wait();
			}
			// Ensure reset of the cancellation token source
			if(!_serializationCancellationTokenSource.TryReset())
			{
				_serializationCancellationTokenSource = new();
			}
		}

		CancellationToken token = _serializationCancellationTokenSource.Token;
		_serializationTask = Task.Run(async () => await _SerializeWithMultipleAttempts(token), token);
	}

	private bool _UpdateCurrentProcessInfoInternal(ProtocolType protocol, int port)
	{
		int currentProcessInfoIndex = Array.FindIndex(Processes, p => p.ProcessId == Environment.ProcessId);
		if(currentProcessInfoIndex < 0)
		{
			// Add this process to the list
			var currentProcessInfo = NekaiProcessNetworkInfo.ForCurrentProcess(protocol, port);
			NekaiProcessNetworkInfo[] updatedProcesses = new NekaiProcessNetworkInfo[Processes.Length + 1];
			Processes.CopyTo(updatedProcesses, 1);
			Processes = updatedProcesses;
			Processes[0] = currentProcessInfo;
		}
		else
		{
			// Update the already existing entry, if necessary
			if(Processes[currentProcessInfoIndex].Protocol == protocol && Processes[currentProcessInfoIndex].Port == port)
				return false;     // Nothing to update
			var currentProcessInfo = NekaiProcessNetworkInfo.ForCurrentProcess(protocol, port);
			Processes[currentProcessInfoIndex] = currentProcessInfo;
		}
		return true;
	}

	// Attempt multiple times, since there might be traffic on the file.
	private async Task _SerializeWithMultipleAttempts(CancellationToken cancellationToken)
	{
		for(int i = 0; i < _SERIALIZATION_MAX_ATTEMPTS; i++)
		{
			if(cancellationToken.IsCancellationRequested)
				return;

			if(TrySerialize().IsSuccess())
				return;
			// File might be locked, so wait a bit before trying again.
			await Task.Delay(_SERIALIZATION_ATTEMPT_DELAY_MS, cancellationToken);
		}
	}

	public void Dispose()
	{
		if(_serializationTask is null)
			return;

		bool isTaskRunning = _serializationTask.Status <= TaskStatus.WaitingForChildrenToComplete;
		if(_serializationTask.Status == TaskStatus.Created)
		{
			// Avoid deadlock in case of a bug
			isTaskRunning = false;
			Exceptor.ThrowIfDebug($"Generated a task for serialization of {nameof(NekaiProcessesNetworkInfo)}, but didn't execute it.");
		}

		if(isTaskRunning && !_serializationCancellationTokenSource.IsCancellationRequested)
		{
			_serializationCancellationTokenSource.Cancel();
			_serializationTask.Wait();  // Wait for the cancellation to take place
		}
		_serializationTask.Dispose();
		_serializationTask = null;
	}
}
