namespace Nekai.Common;

public readonly ref struct PathSpan
{
	public static implicit operator PathSpan(PathString path)
		=> new(path);

	public char this[int index] => _path[index];
	
	private readonly ReadOnlySpan<char> _path;
	public int Length => _path.Length;
	public bool IsEmpty => _path.IsEmpty;
	public bool IsAbsolute => Path.IsPathRooted(_path);
	public bool IsRelative => !IsAbsolute;
	
	// Access to this constructor is required by the PathString.AsSpan() method.
	internal PathSpan(PathString path)
	{
		_path = path.ToString().AsSpan();
	}

	private PathSpan(ReadOnlySpan<char> path)
		=> _path = path;

	public PathSpan ContainingFolder()
	{
		int lastSeparator = _path.LastIndexOfAny('/', '\\');
		if(lastSeparator == -1)
			return this;	// The path is already the root folder.

		return new PathSpan(_path[..lastSeparator]);
	}

	public PathString ToPathString()
		=> new(this);

	public override string ToString() 
		=> _path.ToString();
}
