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

	/// <summary>
	/// Get the containing directory of this path.
	/// </summary>
	/// <returns> A new instance identifying the containing directory if present, or the current instance if it's already the root directory or 
	/// no higher directory is identifiable. </returns>
	public PathSpan ContainingFolder()
	{
		int lastSeparator = Path.LastIndexOfAny('/', '\\');
		if(lastSeparator == -1)
			return this;	// The path is already the root folder.

		return new PathSpan(Path[..lastSeparator]);
	}

	/// <summary>
	/// Generate a new <see cref="PathString"/> object from this instance.
	/// </summary>
	/// <returns> A new <see cref="PathString"/> instance where <see cref="PathString.Path"/> is exactly <see cref="Path"/>. </returns>
	public PathString ToPathString()
		=> new(this);

	public override string ToString() 
		=> Path.ToString();
}
