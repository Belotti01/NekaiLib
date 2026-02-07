namespace Nekai.Common;

public class PathOperationException : Exception
{
	public PathOperationResult Result { get; private set; }
	public string? Path { get; private set; }

	public PathOperationException(PathOperationResult result, string path)
		: base($"{result.GetMessage()}: {path}")
	{
		Result = result;
		Path = path;
	}

	public PathOperationException(PathOperationResult result)
		: base(result.GetMessage())
	{
		Result = result;
	}
}
