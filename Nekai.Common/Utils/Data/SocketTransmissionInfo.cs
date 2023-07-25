using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

/// <summary>
/// Contains information for a multi-part transmission over a socket, and methods to send and receive that information.
/// </summary>
public readonly struct SocketTransmissionInfo
{
	/// <summary> The size of the buffer to use for each fragment. </summary>
	public int BufferSize { get; }
	/// <summary> The total size of the data to be transmitted. </summary>
	public int TotalSize { get; }
	/// <summary> The implied size of the last fragment. </summary>
	public int LastPacketSize
	{
		get
		{
			int size = TotalSize % BufferSize;
			if(size == 0)
				return BufferSize;	// Packets are all the same size:	[-----][-----][-----]
			return size;	// One extra packet is present:				[-----][-----][-----][--]
		}
	}
	/// <summary> The number of fragments that will be sent. </summary>
	public int PacketsCount => (int)Math.Ceiling((double)TotalSize / BufferSize);

	/// <summary>
	/// Create a new <see cref="SocketTransmissionInfo"/> based on the given buffer size and total transfer size.
	/// </summary>
	/// <param name="bufferSize"> The size of the buffer used for the transmission. </param>
	/// <param name="totalSize"> The total size of the data to transfer. </param>
	/// <exception cref="ArgumentOutOfRangeException"> Thrown when either the <paramref name="bufferSize"/> or <paramref name="totalSize"/> are
	/// less or equal than 0. </exception>
	public SocketTransmissionInfo(int bufferSize, int totalSize)
	{
		if(bufferSize <= 0)
			throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be greater than 0.");
		if(totalSize <= 0)
			throw new ArgumentOutOfRangeException(nameof(totalSize), "Total size must be greater than 0.");
		BufferSize = bufferSize;
		TotalSize = totalSize;
	}

	/// <summary> Generates a new instance of <see cref="SocketTransmissionInfo"/> from the <paramref name="bytes"/> array.
	/// The first 4 bytes indicate the buffer size, and the next 4 the total size of the transfer. Any following bytes will be ignored. </summary>
	/// <exception cref="ArgumentNullException"> Thrown when the <paramref name="bytes"/> array is null. </exception>
	/// <exception cref="ArgumentException"> Thrown when the <paramref name="bytes"/> array contains less than 4 <see langword="byte"/> elements. </exception>
	private static SocketTransmissionInfo _FromBytes(byte[] bytes)
	{
		if(bytes is null)
			throw new ArgumentNullException(nameof(bytes));
		if(bytes.Length < 8)
			throw new ArgumentException("Byte array must be at least 8 bytes long.", nameof(bytes));
		
		int bufferSize = BitConverter.ToInt32(bytes, 0);
		int totalSize = BitConverter.ToInt32(bytes, 4);
		return new(bufferSize, totalSize);
	}
	
	/// <summary> Format the data contained in this instance into a <see langword="byte"/> array. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte[] _ToByteArray()
	{
		var bytes = new byte[8];
		BitConverter.GetBytes(BufferSize).CopyTo(bytes, 0);
		BitConverter.GetBytes(TotalSize).CopyTo(bytes, 4);
		return bytes;
	}

	/// <summary>
	/// Asynchronously send the information contained in this instance through the given socket.
	/// </summary>
	/// <param name="socket"> The socket to use to send the information. </param>
	/// <param name="token"> The token used to eventually cancel the operation. </param>
	/// <returns>
	/// <see langword="true"/> if all the information has been sent, <see langword="false"/> if the operation has been cancelled or interrupted.
	/// </returns>
	public async Task<bool> SendAsync(Socket socket, CancellationToken token = default)
	{
		var bytes = _ToByteArray();
		socket.SendBufferSize = bytes.Length;
		int sentBytes = await socket.SendAsync(bytes, token);
		return sentBytes == bytes.Length;
	}

	/// <summary>
	/// Send the information contained in this instance through the given socket.
	/// </summary>
	/// <inheritdoc cref="SendAsync(Socket, CancellationToken)"/>
	public bool Send(Socket socket)
	{
		var bytes = _ToByteArray();
		socket.SendBufferSize = bytes.Length;
		int sentBytes = socket.Send(bytes);
		return sentBytes == bytes.Length;
	}

	/// <summary>
	/// Asynchronously await the reception of a <see cref="SocketTransmissionInfo"/> from the given socket.
	/// </summary>
	/// <param name="socket"> The socket to listen to. </param>
	/// <param name="token"> The token used to eventually interrupt the operation. </param>
	/// <returns> A <see cref="SocketTransmissionInfo"/> containing the information received by the <paramref name="socket"/>. </returns>
	/// <exception cref="ArgumentNullException"> Thrown when <paramref name="socket"/> is null. </exception>
	/// <exception cref="OperationCanceledException"> Thrown when the operation has been cancelled. </exception>
	/// <exception cref="ArgumentOutOfRangeException"> Thrown when the parsed information is not applicable. </exception>
	public static async Task<SocketTransmissionInfo> ReceiveAsync(Socket socket, CancellationToken token = default)
	{
		var bytes = new byte[8];
		socket.ReceiveBufferSize = bytes.Length;
		await socket.ReceiveAsync(bytes, token);
		token.ThrowIfCancellationRequested();
		return _FromBytes(bytes);
	}

	/// <summary>
	/// Wait for the reception of a <see cref="SocketTransmissionInfo"/> from the given socket.
	/// </summary>
	/// <inheritdoc cref="ReceiveAsync(Socket, CancellationToken)"/>
	public static SocketTransmissionInfo Receive(Socket socket)
	{
		var bytes = new byte[8];
		socket.ReceiveBufferSize = bytes.Length;
		socket.Receive(bytes);
		return _FromBytes(bytes);
	}
}
