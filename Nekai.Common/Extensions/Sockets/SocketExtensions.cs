using System.Net.Sockets;
using System.Numerics;
using System.Text.Json;

namespace Nekai.Common;

public static class SocketExtensions
{
	public static async Task<int> SendAsync<T>(this Socket socket, T value) where T : INumber<T>
	{
		Memory<byte> bytes = value.ToByteArray();
		return await socket.SendAsync(bytes);
	}

	public static async Task<NetworkOperationResult> SendJsonAsync(this Socket socket, string json, int maxBufferSize = 1024, CancellationToken token = default)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		int bufferSize = Math.Min(maxBufferSize, bytes.Length);
		SocketTransmissionInfo info = new(bufferSize, bytes.Length);
		await info.SendAsync(socket, token);

		for(int i = 0; i < info.PacketsCount; i++)
		{
			var memory = bytes.AsMemory(i * info.BufferSize);
			await socket.SendAsync(memory, token);

			if(token.IsCancellationRequested)
				return NetworkOperationResult.TimedOut;
		}
		return NetworkOperationResult.Success;
	}

	public static async Task<NetworkOperationResult> SendAsJsonAsync<T>(this Socket socket, T data, int maxBufferSize = 1024, CancellationToken token = default)
	{
		string json = JsonSerializer.Serialize(data);
		return await socket.SendJsonAsync(json, maxBufferSize, token);
	}

	public static async Task<Result<T, NetworkOperationResult>> ReceiveJsonAsync<T>(this Socket socket, CancellationToken token = default)
	{
		SocketTransmissionInfo info = await SocketTransmissionInfo.ReceiveAsync(socket, token);
		var bytes = new byte[info.TotalSize];
		socket.ReceiveBufferSize = info.BufferSize;

		for(int i = 0; i < info.PacketsCount; i++)
		{
			var memory = bytes.AsMemory(i * info.BufferSize);
			await socket.ReceiveAsync(memory, token);

			if(token.IsCancellationRequested)
				return new(NetworkOperationResult.TimedOut);
		}

		T? result = JsonSerializer.Deserialize<T>(bytes);
		if(result is null)
			return new(NetworkOperationResult.BadFormat);
		return new(result);
	}
}