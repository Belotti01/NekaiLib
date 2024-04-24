using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Numerics;

namespace Nekai.Common;

// - No need to implement IEquatable<PathString>, as it's already implemented for strings and the type is implicitly convertible to string (and
// the logic wouldn't change anyway - it should just check the Path property).
// - Implements IParsable to more easily allow deserialization of this type. Using this type over a string allows the prevention of 
// invalid paths in the middle of the program's main logic, as any error is caught during parsing.
// - Does not implement ISpanParsable since the span would have to be converted to a string object anyway for validation.

/// <summary>
/// <see langword="string"/> representing a file path or directory path.
/// </summary>
public class PathString
	: IParsable<PathString>, IComparable<string>, IEquatable<string>, IEqualityOperators<PathString, string, bool>, IEquatable<PathString>
{
	/// <summary> Extracts the contained path as a <see langword="string"/>. </summary>
	/// <param name="path"> The path to extract. </param>
	public static implicit operator string(PathString path)
		=> path.Path;
	// <summary> Extracts the contained path as a <see langword="string"/>. </summary>
	/// <param name="path"> The path to extract. </param>
	public static implicit operator PathString(PathSpan path)
		=> path.ToPathString();
	/// <summary> Extracts the contained path as a <see cref="ReadOnlySpan{T}"/> of <see langword="char"/>. </summary>
	/// <param name="path"> The path to extract. </param>
	public static implicit operator ReadOnlySpan<char>(PathString path)
		=> path.Path.AsSpan();
	/// <inheritdoc cref="AsSpan"/>
	/// <param name="path"> The path to convert. </param>
	// Used for optimization with methods that don't require the string object.
	public static implicit operator PathSpan(PathString path)
		=> new(path);
	/// <inheritdoc cref="AsSpan"/>
	/// <param name="path"> The path to convert. </param>
	public static implicit operator PathString(ReadOnlySpan<char> path)
		=> new(path.ToString());

	/// <summary> Validate the <paramref name="path"/> and wrap it as a <see cref="PathString"/> if successful. Returns <see langword="null"/> 
	/// if the path validation fails. </summary>
	/// <param name="path"> The <see langword="string"/> to convert. </param>
	public static explicit operator PathString?(string? path)
		=> TryParse(path, null, out var result)
		? result
		: null;

	/// <inheritdoc cref="string.this[int]"/>
	public char this[int index] => Path[index];

	/// <summary> The <see langword="string"/> representation of this <see cref="PathString"/>. </summary>
	public string Path { get; }
	public static PathString Empty { get; } = new("");

	public int Length => Path.Length;
	public bool IsEmpty => Path.AsSpan().IsEmpty;
	public bool IsAbsolute => System.IO.Path.IsPathRooted(Path.AsSpan());
	public bool IsRelative => !IsAbsolute;
	public bool IsRootOnly { 
		get {
			var span = Path.AsSpan();
			return System.IO.Path.GetPathRoot(span) == span;
		}
	}
    public static bool operator ==(PathString? left, string? right)
		=> string.Equals(left?.Path, right);

	public static bool operator !=(PathString? left, string? right)
		=> !string.Equals(left?.Path, right);

	public static bool operator ==(PathString? left, ReadOnlySpan<char> right)
		=> left is not null 
		&& left.Equals(right);

	public static bool operator !=(PathString? left, ReadOnlySpan<char> right)
		=> left is null
		|| !left.Equals(right);


    internal PathString(PathSpan path)
		=> Path = path.ToString();

	/// <summary> Used internally to create a new instance. </summary>
	/// <param name="path"> The path to use. </param>
	/// <remarks> Make sure that the <paramref name="path"/> has been validated BEFORE invoking this constructor. </remarks>
	internal PathString(string path)
	{
		Path = path;
	}

	// For more straight-forward "manual" parsing, outside of deserialization libraries.
	public static Result<PathString, PathOperationResult> TryParse([NotNullWhen(true)] string? s, bool keepPathRelative = false)
	{
		var result = NekaiPath.ValidatePath(s);
		if(result.IsSuccessful)
			return new PathString(keepPathRelative ? s! : result.Value);
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
		=> File.Exists(Path);

	/// <summary>
	/// Check whether the path points to an existing directory.
	/// </summary>
	/// <returns> <see langword="true"/> if the path identifies an existing directory, or <see langword="false"/> if the directory
	/// does not exist or the path points to a file instead. </returns>
	public bool IsExistingDirectory()
		=> Directory.Exists(Path);

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
			Directory.CreateDirectory(Path);
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			return NekaiPath.GetResultFromException(ex);
		}
	}

    /// <inheritdoc cref="PathSpan.GetContainingFolder"/>
    /// <remarks> Returns a <see cref="PathSpan"/> rather than a <see cref="PathString"/> to avoid allocating a new object for each call during
    /// loops. </remarks>
    public PathSpan GetContainingFolder()
		=> AsSpan().GetContainingFolder();

    /// <inheritdoc cref="PathSpan.TryGetContainingFolder"/>
	/// <remarks> Returns a <see cref="PathSpan"/> rather than a <see cref="PathString"/> to avoid allocating a new object for each call during
    /// loops. </remarks>
    public bool TryGetContainingFolder(out PathSpan containingFolder)
	{
		containingFolder = GetContainingFolder();
		return Length != containingFolder.Length;
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
			File.Create(Path).Dispose();
			return PathOperationResult.Success;
		}
		catch(Exception ex)
		{
			Debug.Assert(!File.Exists(Path), $"File creation threw \"{ex.GetType().Name}\", but the file exists: {ex.Message}");
			return NekaiPath.GetResultFromException(ex);
		}
	}

	public PathOperationResult EnsureDeletion()
	{
		// Delete File
		if(IsExistingFile())
		{
			try
			{
				File.Delete(this);
				return PathOperationResult.Success;
			}catch(Exception ex)
			{
				return ex switch
				{
					UnauthorizedAccessException => PathOperationResult.NotAllowed,
					_ => PathOperationResult.UnknownFailure
				};
			}
		}

		// Delete Directory
		if(IsExistingDirectory())
		{
			try
			{
				Directory.Delete(this);
				return PathOperationResult.Success;
			}
			catch(Exception ex)
			{
				return ex switch
				{
					UnauthorizedAccessException => PathOperationResult.NotAllowed,
					_ => PathOperationResult.UnknownFailure
				};
			}
		}

		// Didn't exist in the first place.
		return PathOperationResult.Success;
	}

	public Result<PathString, PathOperationResult> TryAppend(string relativePath)
	{
		if(string.IsNullOrWhiteSpace(relativePath))
			return new(PathOperationResult.PathIsEmpty);

		string newPath = System.IO.Path.Combine(Path, relativePath);
        // Don't use the constructor here, since the length and characters of the new path need to be validated.
		return TryParse(newPath);
	}

	public Result<PathString, PathOperationResult> TryAppend(params string[] pathSteps)
    {
        if(pathSteps.Length == 0)
			return new(PathOperationResult.PathIsEmpty);
		
        string relativePath = System.IO.Path.Combine(pathSteps);
        return TryAppend(relativePath);
	}

	/// <summary>
	/// Create a new instance of <see cref="PathString"/> pointing to the directory containing the current path.
	/// </summary>
	/// <returns> A new instance of <see cref="PathString"/> pointing to the directory containing the current path, or the current path if no
	/// containing directory is available (e.g. the path is a root directory). </returns>
	public PathString GetContainingDirectory()
		=> NekaiPath.TryRemovePathStep(Path, out string? result)
			? new(result)	// This is safe since we already validated the path.
			: this;

	/// <summary>
	/// Whether the path is rooted (i.e. starts with a drive letter or a directory separator).
	/// </summary>
	public bool IsRooted()
		=> System.IO.Path.IsPathRooted(Path);


	/// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider?)"/>
	/// <exception cref="FormatException"> Thrown when the path validation fails (see inner exception for more information). </exception>
	public static PathString Parse(string? s, IFormatProvider? provider = null)
	{
		if(string.IsNullOrWhiteSpace(s))
			return Empty;

		try
		{
			string path = System.IO.Path.GetFullPath(s);
			return new(path);
		}catch(Exception ex)
		{
			// Just throw the exception if it fails.
			throw new FormatException($"Failed to parse \"{s}\" as a {nameof(PathString)}.", ex);
		}
    }

    /// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out PathString? result)
    {
        result = null;
        if(string.IsNullOrWhiteSpace(s))
		{
			result = Empty;
			return true;
		}
            

        Result<PathString, PathOperationResult> parseResult = TryParse(s);
		if(!parseResult.IsSuccessful)
			return false;
        
		result = parseResult.Value;
        return true;
    }

	/// <summary>
	/// Get or generate the absolute path of this instance.
	/// </summary>
	/// <param name="basePath"> The beginning of the resulting path. </param>
	/// <returns> This instance if already absolute and <paramref name="basePath"/> is <see langword="null"/>, or
	/// a new fully qualified <see cref="PathString"/> generated from this instance. </returns>
	public PathString ToAbsolutePath(string? basePath = null)
	{
		// Need to fork since the overload with the basePath throws an exception if the basePath is null.
		if(basePath is null)
		{
			if(IsAbsolute)
				return this;
			return new(System.IO.Path.GetFullPath(Path));
		}
		return new(System.IO.Path.GetFullPath(Path, basePath));
    }

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
    public FileAttributes GetFileAttributes()
    {
		if(!IsExistingFile())
			throw new InvalidOperationException("The path points to a directory or a file that does not exist.");

		return File.GetAttributes(Path);
    }

    public bool CanBeReadAsFile()
	{
		try
		{
			using FileStream stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			return true;
        }
        catch
		{
			return false;
		}
    }

    public string GetFileContent()
    {
        return File.ReadAllText(Path);
    }

    public string[] GetFileLines()
    {
        return File.ReadAllLines(Path);
    }

    /// <summary>
    /// Check whether this path is virtually equivalent to <paramref name="other"/>.
    /// </summary>
    /// <param name="other"> The path to compare this instance to. </param>
    /// <returns> <see langword="true"/> if this path points to the same directory or file as <paramref name="other"/>; 
    /// <see langword="false"/> otherwise. </returns>
    public bool Equals(string? other) 
		=> Path.Equals(other);		// TODO: Check OS for case-sensitivity and use the proper comparer.
	
	/// <inheritdoc cref="string.CompareTo(string?)"/>
	public int CompareTo(string? other) 
		=> Path.CompareTo(other);  // TODO: Check OS for case-sensitivity and use the proper comparer.


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
		=> Path.GetHashCode();

	/// <summary>
	/// Get the <see langword="string"/> representation of the contained path.
	/// </summary>
	public override string ToString()
		=> Path;

	/// <inheritdoc cref="Equals(string?)"/>
	public bool Equals(ReadOnlySpan<char> other)
		=> Path.AsSpan() == other;		// TODO: Check OS for case-sensitivity and use the proper comparer.
	
	/// <inheritdoc cref="System.MemoryExtensions.SequenceCompareTo{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
	public int CompareTo(ReadOnlySpan<char> other) 
		=> Path.AsSpan().CompareTo(other, StringComparison.Ordinal);   // TODO: Check OS for case-sensitivity and use the proper comparer.
    public bool Equals(PathString? other) 
		=> string.Equals(Path, other?.Path);
}
