namespace Nekai.Common;

/// <summary>
/// <see cref="ReadOnlySpan{T}"/> of a <see cref="PathString"/>.
/// </summary>
public readonly struct PathSpan
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
	public bool IsRootOnly => System.IO.Path.GetPathRoot(Path) == Path;

	public ReadOnlySpan<char> Root => System.IO.Path.GetPathRoot(Path);

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
	// To avoid allocating a new string each step during iteration, work with spans instead.
	public PathSpan GetContainingFolder()
	{
		int lastSeparator = Path.LastIndexOfAny('/', '\\');

		if(lastSeparator == -1 || IsRootOnly)
			return this;    // The path is already the root folder.

		return new PathSpan(Path[..lastSeparator]);
	}

	/// <summary>
	/// Try to extract the containing directory of this path.
	/// </summary>
	/// <param name="containingFolder"> A new instance identifying the containing directory if present, or the current instance if it's already the root directory or
	/// no higher directory is identifiable. </param>
	/// <returns> <see langword="true"/> if a containing directory is present, or <see langword="false"/> otherwise. </returns>
	// To avoid allocating a new string each step during iteration, work with spans instead.
	public bool TryGetContainingFolder(out PathSpan containingFolder)
	{
		containingFolder = GetContainingFolder();
		return Length != containingFolder.Length;
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