namespace Nekai.Common;

public static class StreamExtensions
{
	/// <summary>
	/// Attempts to write the contents of <paramref name="line"/> onto the <paramref name="stream"/>.
	/// </summary>
	/// <param name="stream">The stream to write to.</param>
	/// <param name="line">The data to write.</param>
	/// <returns><see langword="true"/> if the data has been written.
	/// <see langword="false"/> if the stream is not writable.</returns>
	public static bool WriteLine(this Stream stream, string line)
	{
		if(!stream.CanWrite)
			return false;
		byte[] bytes = Encoding.Default.GetBytes(line + Environment.NewLine);
		stream.Write(bytes, 0, bytes.Length);
		return true;
	}

	/// <inheritdoc cref="WriteLine(System.IO.Stream,string)"/>
	/// <param name="encoding">The encoding of the data.</param>
	public static bool WriteLine(this Stream stream, string line, Encoding encoding)
	{
		if(!stream.CanWrite)
			return false;
		byte[] bytes = encoding.GetBytes(line + Environment.NewLine);
		stream.Write(bytes, 0, bytes.Length);
		return true;
	}

	/// <summary>
	/// Read a single line from the <paramref name="stream"/> and return it as a string.
	/// </summary>
	/// <param name="stream"> The stream to read from. </param>
	/// <returns> The line as a <see langword="string"/> (excluding NewLine characters), or <see langword="null"/>
	/// if EOF has already been reached. </returns>
	/// <exception cref="InvalidOperationException">Thrown if the stream is not readable.</exception>
	public static string? ReadLine(this Stream stream)
	{
		if(!stream.CanRead)
			throw new InvalidOperationException("The stream is not readable.");
		var builder = new StringBuilder();
		int read = stream.ReadByte();
		if(read == -1)  // EOF
			return null;

		do
		{
			if(read == '\r')
			{
				read = stream.ReadByte();
				continue;
			}
			if(read == '\n')
			{
				break;
			}
			builder.Append((char)read);
			read = stream.ReadByte();
		} while(read != -1);

		return builder.ToString();
	}

	/// <inheritdoc cref="ReadAllText(Stream, Encoding, bool)"/>
	public static string? ReadAllText(this Stream stream, bool autoDetectEncoding = false)
	{
		if(!stream.CanRead)
			throw new InvalidOperationException("The stream is not readable.");
		using TextReader reader = new StreamReader(stream, autoDetectEncoding);
		return reader.ReadToEnd();
	}

	/// <summary>
	/// Read all the content of the <paramref name="stream"/> as text and return it as a <see langword="string"/>.
	/// </summary>
	/// <param name="stream"> The stream to read. </param>
	/// <param name="encoding"> The text encoding to use. </param>
	/// <exception cref="InvalidOperationException">Thrown if the stream is not readable.</exception>
	public static string? ReadAllText(this Stream stream, Encoding encoding)
	{
		if(!stream.CanRead)
			throw new InvalidOperationException("The stream is not readable.");
		using TextReader reader = new StreamReader(stream, encoding);
		return reader.ReadToEnd();
	}


	/// <summary>
	/// Read all the content of the <paramref name="stream"/> as an array of bytes.
	/// </summary>
	/// <param name="stream"> The stream to read. </param>
	/// <exception cref="InvalidOperationException">Thrown if the stream is not readable.</exception>
	public static ReadOnlyMemory<byte> ReadAllBytes(this Stream stream)
	{
		if(!stream.CanRead)
			throw new InvalidOperationException("The stream is not readable.");
		IEnumerable<byte> bytes = [];
		int frame;
		
		while((frame = stream.ReadByte()) != -1)
		{
			bytes = bytes.Append((byte)frame);
		}

		return bytes.ToArray();
	}
}