using System;

namespace Nekai.Common;

// The contained path must always be a valid and absolute path.

public class PathString
{
	public static implicit operator PathSpan(PathString path)
		=> new(path);

	private readonly string _path;

	internal PathString(PathSpan path)
		=> _path = path.ToString();

	private PathString(string path)
		=> _path = path;

	public static Result<PathString, PathOperationResult> FromString(string path)
	{
		// TODO: Refactor ValidatePath to return the same type as this method, then adapt this.
		var result = NekaiPath.ValidatePath(path);
		if(result.IsSuccessful)
			return new PathString(path);
		return new(PathOperationResult.UnknownFailure);
	}

	public PathSpan AsSpan()
		=> new(this);

	public override string ToString() 
		=> _path;
}

public readonly ref struct PathSpan
{
	private readonly ReadOnlySpan<char> _path;

	internal PathSpan(PathString path)
		=> _path = path.ToString().AsSpan();

	public PathString ToPath()
		=> new(this);

	public override string ToString() 
		=> _path.ToString();
}
