using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
	: IParsable<PathString>, IComparable<string>, IEquatable<string>, IEqualityOperators<PathString, string, bool>
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

	/// <summary> Validate the <paramref name="path"/> and wrap it as a <see cref="PathString"/> if successful. Throws <see cref="InvalidCastException"/>
	/// if the parsing fails. </summary>
	/// <param name="path"> The <see langword="string"/> to convert. </param>
	/// <exception cref="InvalidCastException"> Thrown when the <see langword="string"/> can't be parsed to a <see cref="PathString"/>. </exception>
	/// <remarks> It's recommended to use the safer <see cref="TryParse(string?, bool)"/> instead. </remarks>
	public static explicit operator PathString(string path) {
		var result = TryParse(path);
		if(result.IsSuccessful)
			return result.Value;

		throw new InvalidCastException($"String casting to PathString failed: {result.Error.GetMessage()}");
	}

	/// <summary>
	/// Append the relative path <paramref name="relativePath"/> to the <paramref name="path"/>.
	/// </summary>
	/// <param name="path">The starting path.</param>
	/// <param name="relativePath">The relative path to append.</param>
	public static PathString operator +(PathString path, string relativePath)
	{
		try
		{
			return (PathString)System.IO.Path.Combine(path, relativePath);
		}
		catch(InvalidCastException ex)
		{
			throw new InvalidOperationException($"The {nameof(relativePath)} is not a valid path string.", ex);
		}
	}

	/// <summary>
	/// Append the relative path <paramref name="relativePath"/> to the <paramref name="path"/>.
	/// </summary>
	/// <param name="path">The starting path.</param>
	/// <param name="relativePath">The relative path to append.</param>
	public static PathString operator +(PathString path, PathString relativePath)
		=> (PathString)System.IO.Path.Combine(path, relativePath);

	/// <inheritdoc cref="string.this[int]"/>
	public char this[int index] => Path[index];

	/// <summary> The <see langword="string"/> representation of this <see cref="PathString"/>. </summary>
	public string Path { get; }

	/// <summary> Instance of <see cref="PathString"/> that represents an empty string. </summary>
	public static PathString Empty { get; } = new("");

	/// <inheritdoc cref="string.Length"/>
	public int Length => Path.Length;
	/// <summary> Whether the <see cref="Path"/> represents an empty string. </summary>
	public bool IsEmpty => Path.AsSpan().IsEmpty;
	/// <inheritdoc cref="System.IO.Path.IsPathRooted(System.ReadOnlySpan{char})"/>
	public bool IsAbsolute => System.IO.Path.IsPathRooted(Path.AsSpan());
	/// Whether the <see cref="Path"/> is not rooted.
	public bool IsRelative => !IsAbsolute;

	/// <summary> Whether the <see cref="Path"/> only represents a root directory. </summary>
	public bool IsRootOnly
	{
		get
		{
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
	/// <inheritdoc cref="IParsable{TSelf}.TryParse"/>
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
		if(!directoryResult.IsSuccessful())
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

	public Result<PathString, PathOperationResult> TryAppend(params string[] pathSteps)
	{
		if(pathSteps.Length == 0)
			return new(PathOperationResult.PathIsEmpty);

		try
		{
			string relativePath = System.IO.Path.Combine(pathSteps);
			return this + relativePath;
		}
		catch(Exception ex)
		{
			return new(PathOperationResult.InvalidPath);
		}
	}

	/// <summary>
	/// Create a new instance of <see cref="PathString"/> pointing to the directory containing the current path.
	/// </summary>
	/// <returns> A new instance of <see cref="PathString"/> pointing to the directory containing the current path, or the current path if no
	/// containing directory is available (e.g. the path is a root directory). </returns>
	public PathString GetContainingDirectory()
		=> NekaiPath.TryRemovePathStep(Path, out string? result)
			? new(result)   // This is safe since we already validated the path.
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
		}
		catch(Exception ex)
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

	/// <returns> <see langword="true"/> if the file was accessed after <paramref name="time"/> amount of time ago; <see langword="false"/> 
	/// otherwise; returns an <see cref="PathOperationResult"/> instead if the access time information cannot be extracted. </returns>
	/// <inheritdoc cref="WasLastAccessedWithin(TimeSpan, bool)"/>
	public Result<bool, PathOperationResult> WasLastAccessedWithin(TimeSpan time)
	{
		if(!IsExistingFile())
			return new(PathOperationResult.DoesNotExist);

		DateTime lastAccessUtc;
		try
		{
			lastAccessUtc = File.GetLastAccessTimeUtc(Path);
		}
		catch(Exception ex)
		{
			// Metadata access denied, or file was not accessible.
			return new(NekaiPath.GetResultFromException(ex));
		}

		bool wasLastAccessInRange = DateTime.UtcNow <= lastAccessUtc + time;
		return wasLastAccessInRange;
	}

	/// <summary>
	/// Checks whether the file was last accessed within <paramref name="time"/>.
	/// </summary>
	/// <param name="time"> The time span after which the file has to have been accessed. </param>
	/// <param name="defaultValue"> The value returned when the required information is not accessible. </param>
	/// <returns> <see langword="true"/> if the file was accessed after <paramref name="time"/> amount of time ago;
	/// <see langword="false"/> otherwise, or <paramref name="defaultValue"/> if the access time information cannot be extracted. </returns>
	public bool WasLastAccessedWithin(TimeSpan time, bool defaultValue)
	{
		var result = WasLastAccessedWithin(time);
		if(!result.IsSuccessful)
			return defaultValue;
		return result.Value;
	}

	/// <summary>
	/// Retrieve the metadata attributes of the file.
	/// </summary>
	/// <returns> A <see cref="FileAttributes"/> composed by the flags identifying the attributes of this file. </returns>
	/// <exception cref="InvalidOperationException"> Thrown when the <see cref="PathString"/> points to a non-existing file, or a directory. </exception>
	public FileAttributes GetFileAttributes()
	{
		if(!IsExistingFile())
			throw new InvalidOperationException("The path points to a directory, or to a file that does not exist.");

		return File.GetAttributes(Path);
	}

	/// <summary>
	/// Attempt to open a <see cref="FileStream"/>.
	/// </summary>
	/// <returns> Whether the operation was successful or not. </returns>
	public bool CanBeReadAsFile()
	{
		if(IsExistingDirectory())
			return false;
		
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

	/// <summary>
	/// Read all contents of the file.
	/// </summary>
	/// <param name="encoding"> The encoding of the contents of the file. </param>
	/// <returns> The contents of the file, or <see langword="null"/> if the file does not exist or is not accessible. </returns>
	public string? ReadFileContent(Encoding? encoding = null)
	{
		try
		{
			return File.ReadAllText(Path, encoding ?? Encoding.Default);
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Couldn't access contents of file '{path}': {exception}", Path, ex.Message);
		}
		return null;
	}

	/// <inheritdoc cref="ReadFileContent(Encoding?)"/>
	public async Task<string?> ReadFileContentAsync(Encoding? encoding = null)
	{
		try
		{
			return await File.ReadAllTextAsync(Path, encoding ?? Encoding.Default);
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Couldn't access contents of file '{path}': {exception}", Path, ex.Message);
		}
		return null;
	}

	/// <inheritdoc cref="ReadFileLines(Encoding?)"/>
	public async Task<string[]?> ReadFileLinesAsync(Encoding? encoding = null)
	{
		try
		{
			return await File.ReadAllLinesAsync(Path, encoding ?? Encoding.Default);
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Couldn't access contents of file '{path}': {exception}", Path, ex.Message);
		}
		return null;
	}

	/// <summary>
	/// Attempt to read all contents of the file.
	/// </summary>
	/// <param name="encoding"> The encoding of the contents of the file. </param>
	/// <returns> The contents of the file, or a <see cref="PathOperationResult"/> identifying an error. </returns>
	public Result<string, PathOperationResult> TryReadFileContent(Encoding? encoding = null)
	{
		try
		{
			return File.ReadAllText(Path, encoding ?? Encoding.Default);
		}
		catch(Exception ex)
		{
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	/// <summary>
	/// Attempt to read all contents of the file.
	/// </summary>
	/// <param name="encoding"> The encoding of the contents of the file. </param>
	/// <returns> The contents of the file, or a <see cref="PathOperationResult"/> identifying an error. </returns>
	public Result<string[], PathOperationResult> TryReadFileLines(Encoding? encoding = null)
	{
		try
		{
			return File.ReadAllLines(Path, encoding ?? Encoding.Default);
		}
		catch(Exception ex)
		{
			return new(NekaiPath.GetResultFromException(ex));
		}
	}

	/// <summary>
	/// Read all lines from the file.
	/// </summary>
	/// <param name="encoding"> The encoding of the contents of the file. </param>
	/// <returns> The lines of the file, or <see langword="null"/> if the file does not exist or is not accessible. </returns>
	public string[]? ReadFileLines(Encoding? encoding = null)
	{
		try
		{
			return File.ReadAllLines(Path, encoding ?? Encoding.Default);
		}
		catch(Exception ex)
		{
			NekaiLogs.Shared.Error("Couldn't access contents of file '{path}': {exception}", Path, ex.Message);
		}
		return null;
	}

	/// <summary>
	/// Extract the file name from this path.
	/// </summary>
	/// <returns> The filename and its extension, or only the filename if <paramref name="keepExtension"/> is <see langword="false"/>. </returns>
	public ReadOnlySpan<char> GetFileOrDirectoryName(bool keepExtension = true)
	{
		// Avoid returning an empty span when possible.
		var span = System.IO.Path.TrimEndingDirectorySeparator(Path.AsSpan());

		return keepExtension
			? System.IO.Path.GetFileName(span)
			: System.IO.Path.GetFileNameWithoutExtension(span);
	}

	/// <inheritdoc cref="Directory.EnumerateFiles(string, string, SearchOption)"/>
	public IEnumerable<string> EnumerateFiles(SearchOption searchOption = SearchOption.TopDirectoryOnly, string searchPattern = "*")
	{
		if(IsExistingFile())
			return [ Path ];

		if(!IsExistingDirectory())
			return [];

		var files = Directory.EnumerateFiles(Path, searchPattern, searchOption);
		
		return files;
	}

	/// <inheritdoc cref="Directory.EnumerateDirectories(string, string, SearchOption)"/>
	public IEnumerable<string> EnumerateDirectories(SearchOption searchOption = SearchOption.TopDirectoryOnly, string searchPattern = "*")
	{
		if(!IsExistingDirectory())
			return [];

		var directories = Directory.EnumerateDirectories(Path, searchPattern, searchOption);

		return directories;
	}

	/// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string, string, SearchOption)"/>
	public IEnumerable<string> EnumerateFileSystemEntries(SearchOption searchOption = SearchOption.TopDirectoryOnly, string searchPattern = "*")
	{
		if(IsExistingFile())
			return [ Path ];

		if(!IsExistingDirectory())
			return [];

		var entries = Directory.EnumerateFileSystemEntries(Path, searchPattern, searchOption);

		return entries;
	}

	/// <summary>
	/// Extract the directory name from this path.
	/// </summary>
	/// <returns> The directory name, or an empty span if only the root is specified. </returns>
	public ReadOnlySpan<char> GetContainingDirectoryName()
	{
		return System.IO.Path.GetDirectoryName(Path.AsSpan());
	}

	public PathString Append(PathString relativePath)
		=> this + relativePath;

	public Result<PathString, PathOperationResult> TryAppend(string relativePath)
	{
		try
		{
			return this + relativePath;
		}
		catch(InvalidOperationException)
		{
			return new(PathOperationResult.InvalidPath);
		}
	}

	/// <summary>
	/// Check whether this path is virtually equivalent to <paramref name="other"/>.
	/// </summary>
	/// <param name="other"> The path to compare this instance to. </param>
	/// <returns> <see langword="true"/> if this path points to the same directory or file as <paramref name="other"/>;
	/// <see langword="false"/> otherwise. </returns>
	public bool Equals(string? other)
		=> Path.Equals(other, StringComparison.CurrentCulture);

	/// <inheritdoc cref="string.CompareTo(string?)"/>
	public int CompareTo(string? other)
		=> String.Compare(Path, other, StringComparison.CurrentCulture);

	public override bool Equals(object? obj)
	{
		if(obj is null)
			return false;

		if(ReferenceEquals(this, obj))
			return true;

		if(!(obj is string or PathString))
			return false;

		return Equals(obj.ToString());
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
		=> Path.AsSpan().Equals(other, StringComparison.CurrentCulture);

	/// <inheritdoc cref="System.MemoryExtensions.SequenceCompareTo{T}(ReadOnlySpan{T}, ReadOnlySpan{T})"/>
	public int CompareTo(ReadOnlySpan<char> other)
		=> Path.AsSpan().CompareTo(other, StringComparison.CurrentCulture);
}