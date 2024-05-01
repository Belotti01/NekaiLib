namespace Nekai.Common;

public static class StreamExtensions
{
	public static bool WriteLine(this Stream stream, string line)
	{
		if(!stream.CanWrite)
			return false;
		byte[] bytes = Encoding.UTF8.GetBytes(line + Environment.NewLine);
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
				continue;
			if(read == '\n')
				break;
			builder.Append((char)read);
			read = stream.ReadByte();
		} while(read != -1);

		return builder.ToString();
    }

    /// <summary>
    /// Read all the content of the <paramref name="stream"/> as text and return it as a <see langword="string"/>.
    /// </summary>
    /// <param name="stream"> The stream to read. </param>
    public static string? ReadAllText(this Stream stream)
    {
        using TextReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}