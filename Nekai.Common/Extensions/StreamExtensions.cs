namespace Nekai.Common;

public static class StreamExtensions
{
	public static bool WriteLine(this Stream stream, string line)
	{
		if(!stream.CanWrite)
			return false;
		byte[] bytes = Encoding.Default.GetBytes(line + Environment.NewLine);
		stream.Write(bytes, 0, bytes.Length);
		return true;
	}

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
	public static string? ReadLine(this Stream stream)
	{
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
		using TextReader reader = new StreamReader(stream, autoDetectEncoding);
		return reader.ReadToEnd();
	}

	/// <summary>
	/// Read all the content of the <paramref name="stream"/> as text and return it as a <see langword="string"/>.
	/// </summary>
	/// <param name="stream"> The stream to read. </param>
	/// <param name="autoDetectEncoding"> Whether to look for byte order marks at the beginning of the file to detect the encoding type. </param>
	/// <param name="encoding"> The text encoding to use. </param>
	public static string? ReadAllText(this Stream stream, Encoding encoding, bool autoDetectEncoding = false)
	{
		using TextReader reader = new StreamReader(stream, encoding);
		return reader.ReadToEnd();
	}


	/// <summary>
	/// Read all the content of the <paramref name="stream"/> as an array of bytes.
	/// </summary>
	/// <param name="stream"> The stream to read. </param>
	public static ReadOnlyMemory<byte> ReadAllBytes(this Stream stream)
	{
		IEnumerable<byte> bytes = [];
		int frame;
		
		while((frame = stream.ReadByte()) != -1)
		{
			bytes = bytes.Append((byte)frame);
		}

		return bytes.ToArray();
	}
}