using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Nekai.Common;

// - No need to implement IEquatable<PathString>, as it's already implemented for strings and the type is implicitly convertible to string (and
// the logic wouldn't change anyway - it should just check the _path member).
// - Implements IParsable to more easily allow deserialization of this type. Using this type over a string allows the prevention of 
// invalid paths in the middle of the program's main logic, as any error is caught during parsing.
// - Does not implement ISpanParsable since the span would have to be converted to a string object anyway.

public class PathString 
	: IParsable<PathString>, IComparable<string>, IEquatable<string>, IEqualityOperators<PathString, string, bool>
{
	/// <summary> Extracts the contained path as a <see langword="string"/>. </summary>
	/// <param name="path"> The path to extract. </param>
	public static implicit operator string(PathString path)
		=> path._path;
	/// <summary> Extracts the contained path as a <see cref="ReadOnlySpan{T}"/> of <see langword="char"/>. </summary>
	/// <param name="path"> The path to extract. </param>
	public static implicit operator ReadOnlySpan<char>(PathString path)
		=> path._path.AsSpan();
	/// <inheritdoc cref="AsSpan"/>
	/// <param name="path"> The path to convert. </param>
	// Used for optimization with methods that don't require the string object.
	public static implicit operator PathSpan(PathString path)
		=> new(path);

	/// <inheritdoc cref="string.this[int]"/>
	public char this[int index] => _path[index];

	private readonly string _path;

	public static bool operator ==(PathString? left, string? right)
		=> string.Equals(left?._path, right);

	public static bool operator !=(PathString? left, string? right)
		=> !string.Equals(left?._path, right);

	public static bool operator ==(PathString? left, ReadOnlySpan<char> right)
		=> left is not null 
		&& left.Equals(right);

	public static bool operator !=(PathString? left, ReadOnlySpan<char> right)
		=> left is null
		|| !left.Equals(right);

	/// <summary> Gets the length of the contained path <see langword="string"/>. </summary>
	public int Length => _path.Length;

	internal PathString(PathSpan path)
		=> _path = path.ToString();

	/// <summary> Used internally to create a new instance. </summary>
	/// <param name="path"> The path to use. </param>
	/// <remarks> Make sure that the <paramref name="path"/> has been validated BEFORE invoking this constructor. </remarks>
	private PathString(string path)
	{
		Debug.Assert(NekaiPath.ValidatePath(path).IsSuccessful);
		_path = path;
	}

	// For more straight-forward "manual" parsing, outside of deserialization libraries.
	public static Result<PathString, PathOperationResult> TryParse(string path, bool keepPathRelative = false)
	{
		var result = NekaiPath.ValidatePath(path);
		if(result.IsSuccessful)
			return new PathString(keepPathRelative ? path : result.Value);
		return new(result.Error);
	}

	/// <summary> Converts the contained path to a <see cref="ReadOnlySpan{T}"/> of <see langword="char"/>. </summary>
	public PathSpan AsSpan()
		=> new(this);

	/// <summary>
	/// Check whether the path points to an existing file.
	/// </summary>
	/// <returns> <see langword="true"/> if the path identifies an existing file, or <see langword="false"/> if the file
	/// does not exist or the path points to a directory instead. </returns>
	public bool IsExistingFile()
		=> File.Exists(_path);

	/// <summary>
	/// Check whether the path points to an existing directory.
	/// </summary>
	/// <returns> <see langword="true"/> if the path identifies an existing directory, or <see langword="false"/> if the directory
	/// does not exist or the path points to a file instead. </returns>
	public bool IsExistingDirectory()
		=> Directory.Exists(_path);

	/// <summary>
	/// Check whether the path points to an existing directory, and if not, try to create it.
	/// </summary>
	/// <returns> <see cref="PathOperationResult.Success"/> if the directory already exists or has been created. </returns>
	public PathOperationResult EnsureExistsAsDirectory()
	{
		if(IsExistingDirectory())
			return PathOperationResult.Success;

		if(IsExistingFile())
			return PathOperationResult.PathIsFile;

		try
		{
			Directory.CreateDirectory(_path);
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			return NekaiPath.GetResultFromException(ex);
		}
	}

	/// <summary>
	/// Check whether the path points to an existing file, and if not, try to create it along with the containing directory.
	/// </summary>
	/// <returns> <see cref="PathOperationResult.Success"/> if the file already exists or has been created, or the value identifying the
	/// error if not. </returns>
	public PathOperationResult EnsureExistsAsFile()
	{
		if(IsExistingFile())
			return PathOperationResult.Success;

		if(IsExistingDirectory())
			return PathOperationResult.PathIsDirectory;

		// First create the directory if it doesn't exist.
		var directory = GetContainingDirectory();
		var directoryResult = directory.EnsureExistsAsDirectory();
		if(!directoryResult.IsSuccess())
			return directoryResult;

		try
		{
			File.Create(_path).Dispose();
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			Debug.Assert(!File.Exists(_path), $"File creation threw \"{ex.GetType().Name}\", but the file exists: {ex.Message}");
			return NekaiPath.GetResultFromException(ex);
		}
	}

	/// <summary>
	/// Create a new instance of <see cref="PathString"/> pointing to the directory containing the current path.
	/// </summary>
	/// <returns> A new instance of <see cref="PathString"/> pointing to the directory containing the current path, or the current path if no
	/// containing directory is available (e.g. the path is a root directory). </returns>
	public PathString GetContainingDirectory()
		=> NekaiPath.TryRemovePathStep(_path, out string? result)
			? new(result)	// This is safe since we already validated the path.
			: this;

	/// <summary>
	/// Whether the path is rooted (i.e. starts with a drive letter or a directory separator).
	/// </summary>
	public bool IsRooted()
		=> Path.IsPathRooted(_path);


	/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
	/// <inheritdoc cref="Path.GetFullPath(string)"/>
	public static PathString Parse(string s, IFormatProvider? provider = null)
	{
		// Just throw the exception if it fails.
		string path = Path.GetFullPath(s);
		return new(path);
	}

	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out PathString? result)
	{
		result = null;
		if(s is null)
			return false;

		var parseResult = TryParse(s);
		if(parseResult.IsSuccessful)
		{
			result = parseResult.Value;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Check whether this path is virtually equivalent to <paramref name="other"/>.
	/// </summary>
	/// <param name="other"> The path to compare this instance to. </param>
	/// <returns> <see langword="true"/> if this path points to the same directory or file as <paramref name="other"/>; 
	/// <see langword="false"/> otherwise. </returns>
	public bool Equals(string? other) 
		=> _path.Equals(other);		// TODO: Check OS for case-sensitivity and use the proper comparer.
	
	/// <inheritdoc cref="string.CompareTo(string?)"/>
	public int CompareTo(string? other) 
		=> _path.CompareTo(other);  // TODO: Check OS for case-sensitivity and use the proper comparer.


	public override bool Equals(object? obj)
	{
		if(obj is null)
			return false;
		
		if(ReferenceEquals(this, obj))
			return true;

		return obj switch
		{
			string s => Equals(s),
			// No need to handle PathString and PathSpan differently since they are string wrappers.
			_ => Equals(obj.ToString())
		};
	}

	/// <summary>
	/// Generate the HashCode of the contained path.
	/// </summary>
	public override int GetHashCode()
		=> _path.GetHashCode();

	/// <summary>
	/// Get the <see langword="string"/> representation of the contained path.
	/// </summary>
	public override string ToString()
		=> _path;

	/// <inheritdoc cref="Equals(string?)"/>
	public bool Equals(ReadOnlySpan<char> other)
		=> _path.AsSpan() == other;		// TODO: Check OS for case-sensitivity and use the proper comparer.
	
	/// <inheritdoc cref="System.MemoryExtensions.SequenceCompareTo{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
	public int CompareTo(ReadOnlySpan<char> other) 
		=> _path.AsSpan().CompareTo(other, StringComparison.Ordinal);   // TODO: Check OS for case-sensitivity and use the proper comparer.
}
