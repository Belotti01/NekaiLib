namespace Nekai.Common;

/// <summary>
/// <see cref="ReadOnlySpan{T}"/> of a <see cref="PathString"/>.
/// </summary>
public readonly ref struct PathSpan
{
	public static implicit operator PathSpan(PathString path)
		=> new(path);

	public static implicit operator ReadOnlySpan<char>(PathSpan path)
		=> path.Path;

	public static explicit operator PathString(PathSpan path)
		=> path.ToPathString();

	public static explicit operator string(PathSpan path)
		=> path.ToString();

	public char this[int index] => Path[index];
	
	public ReadOnlySpan<char> Path { get; }
	public int Length => Path.Length;
	public bool IsEmpty => Path.IsEmpty;
	public bool IsAbsolute => System.IO.Path.IsPathRooted(Path);
	public bool IsRelative => !IsAbsolute;
	
	// Access to this constructor is required by the PathString.AsSpan() method.
	internal PathSpan(PathString path)
	{
		Path = path.ToString().AsSpan();
	}

	private PathSpan(ReadOnlySpan<char> path)
		=> Path = path;

	public PathSpan ContainingFolder()
	{
		int lastSeparator = Path.LastIndexOfAny('/', '\\');
		if(lastSeparator == -1)
			return this;	// The path is already the root folder.

		return new PathSpan(Path[..lastSeparator]);
	}

	public PathString ToPathString()
		=> new(this);

	public override string ToString() 
		=> Path.ToString();
}
